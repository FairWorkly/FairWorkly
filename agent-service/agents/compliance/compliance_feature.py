from typing import Dict, Any
import logging
from master_agent.feature_registry import FeatureBase
from master_agent.config import load_config
from agents.shared.llm.factory import LLMProvider

class ComplianceFeature(FeatureBase):
    """
    Compliance Feature - Fair Work compliance checks
    
    TODO: Implement specific logic by Albert
    Required functionalities:
    1. Fair Work Q&A (call existing RAG + LLM)
    2. Roster compliance checks
    """
    
    def __init__(self):
        self.logger = logging.getLogger(__name__)
        config = load_config()
        self.system_prompt = config.get("prompt", {}).get(
            "system_prompt",
            "You are FairWorkly's compliance co-pilot. Answer using Fair Work context.",
        )
        try:
            self.llm = LLMProvider()
        except Exception as exc:
            self.logger.warning("LLM provider unavailable: %s", exc)
            self.llm = None
    
    async def process(self, payload: Dict[str, Any]) -> Dict[str, Any]:
        """
        Process Compliance requests
        
        Args:
            payload: {
                'message': str,      # User message
                'file_name': str,    # File name (if any)
                'file': UploadFile   # File object (if any)
            }
        
        Returns:
            dict: Processing result
        """
        message = (payload.get('message') or '').strip()
        file_name = payload.get('file_name')
        
        if not message:
            return {
                "type": "compliance",
                "message": "Please provide a question for compliance assistance.",
                "file_name": file_name,
                "note": "MESSAGE_REQUIRED"
            }
        
        if not self.llm:
            # Fallback placeholder when LLM is unavailable
            return {
                "type": "compliance",
                "message": f"Compliance Feature placeholder - Received message: {message}",
                "file_name": file_name,
                "note": "LLM provider not configured"
            }
        
        messages = [
            {"role": "system", "content": self.system_prompt},
            {"role": "user", "content": message},
        ]
        
        try:
            llm_response = await self.llm.generate(messages, temperature=0.3, max_tokens=800)
            answer = llm_response.get("content", "").strip()
        except Exception as exc:
            self.logger.error("LLM call failed: %s", exc, exc_info=True)
            return {
                "type": "compliance",
                "message": f"Compliance Feature placeholder - Received message: {message}",
                "file_name": file_name,
                "note": "LLM invocation failed"
            }
        
        return {
            "type": "compliance",
            "message": answer or "No answer generated.",
            "file_name": file_name,
            "model": llm_response.get("model"),
        }
