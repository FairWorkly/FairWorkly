"""LLM Provider factory that reads defaults from config/environment."""

import os
from typing import Optional, List, Dict, Any

from master_agent.config import load_config
from .provider_base import LLMProviderBase
from .langchain_provider import LangChainOpenAIProvider
from .local_provider import LocalHuggingFaceProvider


class LLMProviderFactory:
    """
    Factory to create the right LLM provider
    """
    
    @staticmethod
    def create(provider_type: Optional[str] = None) -> LLMProviderBase:
        """
        Create LLM provider based on config
        """
        config = load_config()
        default_provider = config.get("model_params", {}).get("deployment_mode_llm", "openai")
        provider_type = provider_type or os.getenv("LLM_PROVIDER", default_provider)
        
        if provider_type == "openai":
            model = os.getenv("OPENAI_MODEL")
            return LangChainOpenAIProvider(model=model)
        if provider_type == "local":
            return LocalHuggingFaceProvider(config)
        
        raise ValueError(f"Unknown provider type: {provider_type}")


class LLMProvider:
    """Simple wrapper that delegates to the factory."""
    
    def __init__(self):
        self.provider = LLMProviderFactory.create()
    
    async def generate(
        self, 
        messages: List[Dict[str, str]], 
        temperature: float = 0.7,
        max_tokens: int = 4096
    ) -> Dict[str, Any]:
        return await self.provider.generate(messages, temperature, max_tokens)
