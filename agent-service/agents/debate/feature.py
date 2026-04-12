"""DebateFeature — multi-agent compliance debate orchestrator.

Three agents evaluate a shift scenario in sequence:
  1. Roster Agent  — surface-level pay-rate assessment
  2. Payroll Agent — challenges/confirms with Award detail + cumulative hours (RAG)
  3. Fair Bot      — final arbitration with Award citation (RAG)
"""

import asyncio
import logging
import re
import time
from pathlib import Path
from typing import Any, Dict, List, Optional, Type

from pydantic import BaseModel, ValidationError

from master_agent.config import load_config
from master_agent.feature_registry import FeatureBase
from shared.llm.factory import LLMProviderFactory
from shared.rag.retriever_manager import ensure_retriever

from .models import (
    DebateResult,
    DebateRound,
    FairBotAgentResponse,
    PayrollAgentResponse,
    RosterAgentResponse,
    ShiftScenario,
)

_PROMPTS_DIR = Path(__file__).parent / "prompts"
_LLM_TIMEOUT = 30

logger = logging.getLogger(__name__)
_NO_RELEVANT_DOCS = "No relevant documents found."


def _load_prompt(name: str) -> str:
    return (_PROMPTS_DIR / f"{name}.txt").read_text(encoding="utf-8")


def _scenario_text(s: ShiftScenario) -> str:
    lines = [
        f"Employee: {s.employee_name}",
        f"Shift date: {s.shift_date}",
        f"Shift hours: {s.shift_hours}",
        f"Hours already worked this week (before this shift): {s.week_hours_before_shift}",
        f"Applicable Award: {s.award_name}",
    ]
    if s.extra_context:
        lines.append(f"Additional context: {s.extra_context}")
    return "\n".join(lines)


def _extract_json_object(text: str) -> str:
    """Extract a single JSON object from the LLM response."""
    stripped = text.strip()
    if stripped.startswith("```"):
        stripped = re.sub(r"^```(?:json)?\s*", "", stripped, flags=re.IGNORECASE)
        stripped = re.sub(r"\s*```$", "", stripped)
        stripped = stripped.strip()

    start = stripped.find("{")
    end = stripped.rfind("}")
    if start == -1 or end == -1 or end < start:
        raise ValueError("Response did not contain a JSON object.")

    return stripped[start : end + 1]


def _parse_agent_response(
    text: str, model_type: Type[BaseModel], agent_name: str
) -> BaseModel:
    """Validate a structured JSON response from a debate agent."""
    try:
        return model_type.model_validate_json(_extract_json_object(text))
    except (ValidationError, ValueError) as exc:
        logger.warning(
            "Invalid structured debate response from %s",
            agent_name,
            exc_info=True,
        )
        raise ValueError(
            f"{agent_name} returned an invalid structured response."
        ) from exc


def _normalize_cited_section(cited_section: str) -> Optional[str]:
    normalized = cited_section.strip()
    if not normalized or normalized.lower().startswith("none"):
        return None
    return normalized


def _has_award_evidence(award_excerpts: str) -> bool:
    normalized = award_excerpts.strip()
    return bool(normalized) and normalized != _NO_RELEVANT_DOCS


def _build_insufficient_evidence_result(
    scenario: ShiftScenario,
    scenario_str: str,
    rag_sources: List[Dict[str, Any]],
) -> DebateResult:
    reasoning = (
        f"FairWorkly could not retrieve usable excerpts from {scenario.award_name} that directly "
        "support the required rate tiers or overtime thresholds for this scenario. The debate was "
        "stopped to avoid unsupported payroll guidance. Retrieve the relevant Award clauses and rerun "
        "the analysis."
    )
    return DebateResult(
        scenario_summary=scenario_str,
        rounds=[
            DebateRound(
                agent="Fair Bot",
                role="Compliance Arbitrator",
                icon="⚖️",
                stance="Insufficient retrieved Award evidence to determine the exact pay rate.",
                reasoning=reasoning,
                sources=rag_sources,
            )
        ],
        final_ruling=(
            "Insufficient retrieved Award evidence to determine the exact pay rate. "
            "Retrieve the relevant Award clauses and rerun this debate."
        ),
        cited_award_section=None,
        model=None,
    )


def _parse_scenario_payload(payload: Dict[str, Any]) -> ShiftScenario:
    scenario_payload = payload.get("scenario")
    if scenario_payload is None:
        raise ValueError("Debate payload must include a 'scenario' object.")
    if not isinstance(scenario_payload, dict):
        raise ValueError("Debate payload 'scenario' must be an object.")

    try:
        return ShiftScenario(**scenario_payload)
    except ValidationError as exc:
        raise ValueError(f"Invalid debate scenario payload: {exc}") from exc


class DebateFeature(FeatureBase):
    """Orchestrates a three-round multi-agent compliance debate."""

    async def process(self, payload: Dict[str, Any]) -> Dict[str, Any]:
        scenario = _parse_scenario_payload(payload)
        config = load_config()
        start = time.perf_counter()

        scenario_str = _scenario_text(scenario)

        # --- Retrieve Award excerpts once (shared by Payroll Agent + Fair Bot) ---
        award_excerpts = ""
        rag_sources: List[Dict[str, Any]] = []
        try:
            retriever, _ = ensure_retriever(config, logger)
            query_parts = [
                scenario.award_name,
                scenario.shift_date,
                f"{scenario.shift_hours} hour shift",
                f"{scenario.week_hours_before_shift} hours already worked this week",
                "penalty rates overtime thresholds",
            ]
            if scenario.extra_context:
                query_parts.append(scenario.extra_context)
            query = " ".join(str(part) for part in query_parts if part)
            top_k = config.get("faiss", {}).get("similarity_search_k_docs", 3)
            retrieval = await retriever.retrieve(query, top_k=top_k)
            award_excerpts = retriever.format_context_for_llm(retrieval)
            rag_sources = retrieval.metadatas
        except Exception:
            logger.warning(
                "RAG retrieval failed for debate; returning insufficient-evidence fallback",
                exc_info=True,
            )

        if not _has_award_evidence(award_excerpts):
            logger.info("Debate skipped due to missing usable Award excerpts")
            return _build_insufficient_evidence_result(
                scenario=scenario,
                scenario_str=scenario_str,
                rag_sources=rag_sources,
            ).model_dump()

        llm = LLMProviderFactory.create()
        model_name: Optional[str] = None
        rounds: List[DebateRound] = []

        # ── Round 1: Roster Agent ────────────────────────────────────
        roster_prompt = _load_prompt("roster_agent").format(
            scenario=scenario_str,
            award_name=scenario.award_name,
            award_excerpts=award_excerpts,
        )
        roster_messages = [
            {"role": "system", "content": roster_prompt},
            {"role": "user", "content": "Review the retrieved Award excerpts and provide your initial assessment."},
        ]
        roster_raw = await self._call_llm(llm, roster_messages, agent_name="Roster Agent")
        model_name = roster_raw.get("model")
        roster_response = _parse_agent_response(
            roster_raw.get("content", ""),
            RosterAgentResponse,
            "Roster Agent",
        )

        rounds.append(DebateRound(
            agent="Roster Agent",
            role="Shift Analyst",
            icon="📋",
            stance=roster_response.stance,
            reasoning=roster_response.reasoning,
            sources=rag_sources,
        ))

        # ── Round 2: Payroll Agent ───────────────────────────────────
        payroll_prompt = _load_prompt("payroll_agent").format(
            scenario=scenario_str,
            roster_opinion=roster_response.model_dump_json(indent=2),
            award_name=scenario.award_name,
            award_excerpts=award_excerpts or "(No Award excerpts available)",
        )
        payroll_messages = [
            {"role": "system", "content": payroll_prompt},
            {"role": "user", "content": "Please review the Roster Agent's assessment and provide your analysis."},
        ]
        payroll_raw = await self._call_llm(llm, payroll_messages, agent_name="Payroll Agent")
        payroll_response = _parse_agent_response(
            payroll_raw.get("content", ""),
            PayrollAgentResponse,
            "Payroll Agent",
        )

        rounds.append(DebateRound(
            agent="Payroll Agent",
            role="Payroll Compliance Specialist",
            icon="💰",
            stance=payroll_response.stance,
            reasoning=payroll_response.reasoning,
            challenges=payroll_response.challenges,
            sources=rag_sources,
        ))

        # ── Round 3: Fair Bot ────────────────────────────────────────
        fairbot_prompt = _load_prompt("fairbot_agent").format(
            scenario=scenario_str,
            roster_opinion=roster_response.model_dump_json(indent=2),
            payroll_opinion=payroll_response.model_dump_json(indent=2),
            award_name=scenario.award_name,
            award_excerpts=award_excerpts or "(No Award excerpts available)",
        )
        fairbot_messages = [
            {"role": "system", "content": fairbot_prompt},
            {"role": "user", "content": "Please deliver your final ruling on this compliance question."},
        ]
        fairbot_raw = await self._call_llm(llm, fairbot_messages, agent_name="Fair Bot")
        fairbot_response = _parse_agent_response(
            fairbot_raw.get("content", ""),
            FairBotAgentResponse,
            "Fair Bot",
        )

        rounds.append(DebateRound(
            agent="Fair Bot",
            role="Compliance Arbitrator",
            icon="⚖️",
            stance=fairbot_response.ruling,
            reasoning=fairbot_response.reasoning,
            agrees_with=fairbot_response.agrees_with,
            sources=rag_sources,
        ))

        cited_section = _normalize_cited_section(fairbot_response.cited_section)

        elapsed = time.perf_counter() - start
        logger.info("Debate completed in %.2fs (%d rounds)", elapsed, len(rounds))

        result = DebateResult(
            scenario_summary=scenario_str,
            rounds=rounds,
            final_ruling=fairbot_response.ruling,
            cited_award_section=cited_section or None,
            model=model_name,
        )
        return result.model_dump()

    async def _call_llm(
        self, llm, messages: List[Dict[str, str]], agent_name: str
    ) -> Dict[str, Any]:
        try:
            return await asyncio.wait_for(
                llm.generate(messages, temperature=0.4, max_tokens=600),
                timeout=_LLM_TIMEOUT,
            )
        except asyncio.TimeoutError:
            logger.error("%s timed out after %ds", agent_name, _LLM_TIMEOUT)
            raise ValueError(
                f"{agent_name} timed out before returning a valid debate response."
            ) from None
        except Exception as exc:
            logger.exception("LLM call failed in debate for %s", agent_name)
            detail = str(exc).strip()
            if detail:
                raise ValueError(
                    f"{agent_name} failed while generating the debate response: {detail}"
                ) from exc
            raise ValueError(
                f"{agent_name} was unavailable while generating the debate response."
            ) from None
