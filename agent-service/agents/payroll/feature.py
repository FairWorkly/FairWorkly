"""PayrollFeature — explains a single payroll compliance issue using RAG + LLM."""

import asyncio
import json
import logging
import time
from pathlib import Path
from typing import Any, Dict, List

from master_agent.config import load_config
from master_agent.feature_registry import FeatureBase
from shared.llm.factory import LLMProvider
from shared.rag.retriever_manager import ensure_retriever

_PROMPTS_DIR = Path(__file__).parent / "prompts"


class PayrollFeature(FeatureBase):
    """Explain a payroll compliance issue with Award-backed context."""

    async def process(self, payload: Dict[str, Any]) -> Dict[str, Any]:
        logger = logging.getLogger(__name__)
        config = load_config()
        start_time = time.time()

        # Log 1: Request received
        logger.info(
            "Payroll explain request: issueId=%s categoryType=%s",
            payload.get("issueId"),
            payload.get("categoryType"),
        )

        # --- Phase 1: Input + retrieval (no retry) ---
        try:
            # 1. Build RAG query
            category_type = payload["categoryType"]
            context_label = payload["description"]["contextLabel"]
            query = f"{category_type} {context_label}"

            # 2. Retrieve Award excerpts
            top_k = config.get("faiss", {}).get("similarity_search_k_docs", 3)
            retriever, _vectorstore = ensure_retriever(config, logger)
            retrieval_result = await retriever.retrieve(query, top_k=top_k)
            award_excerpts = retriever.format_context_for_llm(retrieval_result)

            # Log 3: FAISS retrieval complete
            pages = [m.get("page") for m in retrieval_result.metadatas]
            logger.info(
                "FAISS retrieval: %d docs, pages=%s",
                len(retrieval_result.metadatas),
                pages,
            )

            # 3. Build prompts
            system_prompt = (_PROMPTS_DIR / "system.txt").read_text(encoding="utf-8")
            user_template = (_PROMPTS_DIR / "user.txt").read_text(encoding="utf-8")
            user_message = user_template.replace(
                "{issue_json}", json.dumps(payload, indent=2)
            ).replace("{award_excerpts}", award_excerpts)
            messages = [
                {"role": "system", "content": system_prompt},
                {"role": "user", "content": user_message},
            ]
        except KeyError as e:
            # Log 2: Input validation failed (defensive — Pydantic covers this)
            logger.warning("Missing field in payload: %s", e)
            return {"code": 400, "msg": "Invalid issue JSON"}
        except Exception:
            logger.exception("Internal error during retrieval phase")
            return {"code": 500, "msg": "Internal processing error"}

        # --- Phase 2: LLM call + split (with retry) ---
        result = await self._call_llm_with_retry(
            messages,
            retrieval_result.metadatas,
            retrieval_result.documents,
            logger,
        )

        # Log 9: Total elapsed time on success
        if result.get("code") == 200:
            elapsed = time.time() - start_time
            logger.info("Payroll explain completed in %.2fs", elapsed)

        return result

    async def _call_llm_with_retry(
        self,
        messages: List[Dict[str, str]],
        sources: List[Dict[str, Any]],
        documents: List[str],
        logger: logging.Logger,
        max_attempts: int = 3,
    ) -> Dict[str, Any]:
        delays = [1, 2]
        last_code = 500
        last_msg = "Internal processing error"

        for attempt in range(max_attempts):
            try:
                # Log 4: LLM call start
                logger.info("LLM call attempt %d/%d", attempt + 1, max_attempts)
                llm_start = time.time()

                llm = LLMProvider()
                llm_response = await llm.generate(
                    messages,
                    temperature=0.3,
                    max_tokens=800,
                )
                raw_text = llm_response.get("content", "").strip()

                # Log 5: LLM returned
                llm_elapsed = time.time() - llm_start
                logger.info(
                    "LLM responded in %.2fs, model=%s",
                    llm_elapsed,
                    llm_response.get("model"),
                )

                # Split LLM output into two sections
                parts = raw_text.split("\n\nRecommendation:\n", 1)
                if len(parts) != 2:
                    raise ValueError(
                        "LLM output missing expected "
                        "'\\n\\nRecommendation:\\n' separator"
                    )
                detailed = parts[0].removeprefix("DetailedExplanation:\n")
                recommendation = parts[1]

                return {
                    "code": 200,
                    "msg": "OK",
                    "data": {
                        "type": "payroll_explain",
                        "detailedExplanation": detailed,
                        "recommendation": recommendation,
                        "model": llm_response.get("model"),
                        "sources": [
                            {**meta, "content": doc}
                            for meta, doc in zip(sources, documents)
                        ],
                        "note": None,
                    },
                }
            except ValueError as e:
                # 502: LLM responded but output unparseable
                last_code = 502
                last_msg = "LLM returned unparseable response"
                # Log 6: Split failure
                logger.error(
                    "LLM output split failed: %s — raw (first 200 chars): %s",
                    e,
                    raw_text[:200],
                )
            except Exception as e:
                # 503: LLM call failed (network, timeout, API key, etc.)
                last_code = 503
                last_msg = "LLM service unavailable"
                logger.exception("LLM call failed: %s", e)

            if attempt < max_attempts - 1:
                # Log 7: Retry
                logger.warning(
                    "Retrying (%d/%d) after %s (code=%d)",
                    attempt + 2,
                    max_attempts,
                    last_msg,
                    last_code,
                )
                await asyncio.sleep(delays[attempt])

        # Log 8: All retries exhausted
        logger.error(
            "All %d attempts failed, returning code=%d",
            max_attempts,
            last_code,
        )
        return {"code": last_code, "msg": last_msg}
