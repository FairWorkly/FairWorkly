"""PayrollFeature — explains a single payroll compliance issue using RAG + LLM."""

import json
import logging
from pathlib import Path
from typing import Any, Dict

from master_agent.config import load_config
from master_agent.feature_registry import FeatureBase
from shared.llm.factory import LLMProvider
from shared.rag.retriever_manager import ensure_retriever

_PROMPTS_DIR = Path(__file__).parent / "prompts"


class PayrollFeature(FeatureBase):
    """Explain a payroll compliance issue with Award-backed context."""

    async def process(self, payload: Dict[str, Any]) -> Dict[str, Any]:
        config = load_config()
        logger = logging.getLogger(__name__)

        # 1. Build RAG query
        category_type = payload["categoryType"]
        context_label = payload["description"]["contextLabel"]
        query = f"{category_type} {context_label}"

        # 2. Retrieve Award excerpts
        top_k = config.get("faiss", {}).get("similarity_search_k_docs", 3)
        retriever, _vectorstore = ensure_retriever(config, logger)
        retrieval_result = await retriever.retrieve(query, top_k=top_k)
        award_excerpts = retriever.format_context_for_llm(retrieval_result)

        # 3. Build prompts
        system_prompt = (_PROMPTS_DIR / "system.txt").read_text(encoding="utf-8")
        user_template = (_PROMPTS_DIR / "user.txt").read_text(encoding="utf-8")
        user_message = (
            user_template
            .replace("{issue_json}", json.dumps(payload, indent=2))
            .replace("{award_excerpts}", award_excerpts)
        )

        # 4. Call LLM
        messages = [
            {"role": "system", "content": system_prompt},
            {"role": "user", "content": user_message},
        ]
        llm = LLMProvider()
        llm_response = await llm.generate(messages, temperature=0.3, max_tokens=800)
        raw_text = llm_response.get("content", "").strip()

        # 5. Split LLM output into two sections
        parts = raw_text.split("\n\nRecommendation:\n", 1)
        if len(parts) != 2:
            raise ValueError(
                "LLM output missing expected '\\n\\nRecommendation:\\n' separator"
            )
        detailed = parts[0].removeprefix("DetailedExplanation:\n")
        recommendation = parts[1]

        # 6. Return
        return {
            "code": 200,
            "msg": "OK",
            "data": {
                "type": "payroll_explain",
                "detailedExplanation": detailed,
                "recommendation": recommendation,
                "model": llm_response.get("model"),
                "sources": retrieval_result.metadatas,
                "note": None,
            },
        }
