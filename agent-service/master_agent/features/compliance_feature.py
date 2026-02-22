from typing import Dict, Any
import logging

from master_agent.feature_registry import FeatureBase
from master_agent.config import load_config
from shared.rag.rag_client import run as run_rag


class ComplianceFeature(FeatureBase):
    """Compliance Feature - Fair Work compliance Q&A via RAG."""

    def __init__(self):
        self.logger = logging.getLogger(__name__)
        self.config = load_config()
        self.system_prompt = self.config.get("prompt", {}).get(
            "system_prompt",
            "You are FairWorkly's compliance co-pilot. Answer using Fair Work context.",
        )

    async def process(self, payload: Dict[str, Any]) -> Dict[str, Any]:
        """Process compliance Q&A requests."""
        message = (payload.get("message") or "").strip()
        file_name = payload.get("file_name")

        if not message:
            return {
                "type": "compliance",
                "message": "Please provide a question for compliance assistance.",
                "file_name": file_name,
                "note": "MESSAGE_REQUIRED",
            }

        rag_result = await run_rag(
            message,
            system_prompt=self.system_prompt,
            config=self.config,
            logger=self.logger,
        )

        return {
            "type": "compliance",
            "message": rag_result.get("content", "No answer generated."),
            "file_name": file_name,
            "model": rag_result.get("model"),
            "sources": rag_result.get("sources", []),
            "note": rag_result.get("note"),
        }