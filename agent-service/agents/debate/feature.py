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
from typing import Any, Dict, List, Optional

from master_agent.config import load_config
from master_agent.feature_registry import FeatureBase
from shared.llm.factory import LLMProviderFactory
from shared.rag.retriever_manager import ensure_retriever

from .models import DebateResult, DebateRound, ShiftScenario

_PROMPTS_DIR = Path(__file__).parent / "prompts"
_LLM_TIMEOUT = 30

logger = logging.getLogger(__name__)


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


def _parse_field(text: str, field: str) -> str:
    """Extract a labelled field value from structured LLM output."""
    pattern = rf"^{field}:\s*(.+?)(?=\n[A-Z_]+:|$)"
    match = re.search(pattern, text, re.MULTILINE | re.DOTALL)
    return match.group(1).strip() if match else ""


class DebateFeature(FeatureBase):
    """Orchestrates a three-round multi-agent compliance debate."""

    async def process(self, payload: Dict[str, Any]) -> Dict[str, Any]:
        scenario = ShiftScenario(**payload["scenario"])
        config = load_config()
        start = time.perf_counter()

        scenario_str = _scenario_text(scenario)

        # --- Retrieve Award excerpts once (shared by Payroll Agent + Fair Bot) ---
        award_excerpts = ""
        rag_sources: List[Dict[str, Any]] = []
        try:
            retriever, _ = ensure_retriever(config, logger)
            query = f"{scenario.award_name} overtime penalty rates Saturday {scenario.extra_context or ''}"
            top_k = config.get("faiss", {}).get("similarity_search_k_docs", 3)
            retrieval = await retriever.retrieve(query, top_k=top_k)
            award_excerpts = retriever.format_context_for_llm(retrieval)
            rag_sources = retrieval.metadatas
        except Exception:
            logger.warning("RAG retrieval failed for debate; proceeding without Award excerpts", exc_info=True)

        llm = LLMProviderFactory.create()
        model_name: Optional[str] = None
        rounds: List[DebateRound] = []

        # ── Round 1: Roster Agent ────────────────────────────────────
        roster_prompt = _load_prompt("roster_agent")
        roster_messages = [
            {"role": "system", "content": roster_prompt},
            {"role": "user", "content": f"Shift Scenario:\n{scenario_str}"},
        ]
        roster_raw = await self._call_llm(llm, roster_messages)
        model_name = roster_raw.get("model")
        roster_text = roster_raw.get("content", "")

        rounds.append(DebateRound(
            agent="Roster Agent",
            role="Shift Analyst",
            icon="📋",
            stance=_parse_field(roster_text, "STANCE"),
            reasoning=_parse_field(roster_text, "REASONING"),
        ))

        # ── Round 2: Payroll Agent ───────────────────────────────────
        payroll_prompt = _load_prompt("payroll_agent").format(
            scenario=scenario_str,
            roster_opinion=roster_text,
            award_name=scenario.award_name,
            award_excerpts=award_excerpts or "(No Award excerpts available)",
        )
        payroll_messages = [
            {"role": "system", "content": payroll_prompt},
            {"role": "user", "content": "Please review the Roster Agent's assessment and provide your analysis."},
        ]
        payroll_raw = await self._call_llm(llm, payroll_messages)
        payroll_text = payroll_raw.get("content", "")

        rounds.append(DebateRound(
            agent="Payroll Agent",
            role="Payroll Compliance Specialist",
            icon="💰",
            stance=_parse_field(payroll_text, "STANCE"),
            reasoning=_parse_field(payroll_text, "REASONING"),
            challenges=_parse_field(payroll_text, "CHALLENGES"),
            sources=rag_sources,
        ))

        # ── Round 3: Fair Bot ────────────────────────────────────────
        fairbot_prompt = _load_prompt("fairbot_agent").format(
            scenario=scenario_str,
            roster_opinion=roster_text,
            payroll_opinion=payroll_text,
            award_name=scenario.award_name,
            award_excerpts=award_excerpts or "(No Award excerpts available)",
        )
        fairbot_messages = [
            {"role": "system", "content": fairbot_prompt},
            {"role": "user", "content": "Please deliver your final ruling on this compliance question."},
        ]
        fairbot_raw = await self._call_llm(llm, fairbot_messages)
        fairbot_text = fairbot_raw.get("content", "")

        rounds.append(DebateRound(
            agent="Fair Bot",
            role="Compliance Arbitrator",
            icon="⚖️",
            stance=_parse_field(fairbot_text, "RULING"),
            reasoning=_parse_field(fairbot_text, "REASONING"),
            challenges=_parse_field(fairbot_text, "AGREES_WITH"),
            sources=rag_sources,
        ))

        cited_section = _parse_field(fairbot_text, "CITED_SECTION")

        elapsed = time.perf_counter() - start
        logger.info("Debate completed in %.2fs (%d rounds)", elapsed, len(rounds))

        result = DebateResult(
            scenario_summary=scenario_str,
            rounds=rounds,
            final_ruling=_parse_field(fairbot_text, "RULING"),
            cited_award_section=cited_section or None,
            model=model_name,
        )
        return result.model_dump()

    async def _call_llm(
        self, llm, messages: List[Dict[str, str]]
    ) -> Dict[str, Any]:
        try:
            return await asyncio.wait_for(
                llm.generate(messages, temperature=0.4, max_tokens=600),
                timeout=_LLM_TIMEOUT,
            )
        except asyncio.TimeoutError:
            logger.error("LLM call timed out after %ds", _LLM_TIMEOUT)
            return {"content": "(Agent timed out)", "model": None}
        except Exception:
            logger.exception("LLM call failed in debate")
            return {"content": "(Agent unavailable)", "model": None}
