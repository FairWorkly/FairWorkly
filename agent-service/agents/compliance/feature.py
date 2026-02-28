from typing import Dict, Any
import logging

from master_agent.feature_registry import FeatureBase, feature_response
from master_agent.config import load_config
from shared.rag.rag_client import run as run_rag
from .prompt_builder import CompliancePromptBuilder


class ComplianceFeature(FeatureBase):
    """Compliance Feature - Fair Work compliance Q&A via RAG."""

    def __init__(self):
        self.logger = logging.getLogger(__name__)
        self.config = load_config()
        self.prompt_builder = CompliancePromptBuilder()

    async def process(self, payload: Dict[str, Any]) -> Dict[str, Any]:
        """Process compliance Q&A requests."""
        message = (payload.get("message") or "").strip()

        if not message:
            return feature_response(
                type="compliance",
                message="Please provide a question for compliance assistance.",
                note="MESSAGE_REQUIRED",
            )

        system_prompt = self.prompt_builder.build_system_prompt()

        rag_result = await run_rag(
            message,
            system_prompt=system_prompt,
            config=self.config,
            logger=self.logger,
            history_messages=payload.get("history_payload"),
        )

        return feature_response(
            type="compliance",
            message=rag_result.get("content", "No answer generated."),
            model=rag_result.get("model"),
            sources=rag_result.get("sources", []),
            note=rag_result.get("note"),
        )
