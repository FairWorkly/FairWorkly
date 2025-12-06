"""
LLM Provider Factory

TODO: Create the right LLM provider based on config.

This decides whether to use Claude or Azure OpenAI.
Reads from environment variables.
"""

import os
from typing import Optional
from .provider_base import LLMProviderBase
from .anthropic_provider import AnthropicProvider
from .azure_openai_provider import AzureOpenAIProvider


class LLMProviderFactory:
    """
    Factory to create the right LLM provider
    """
    
    @staticmethod
    def create(provider_type: Optional[str] = None) -> LLMProviderBase:
        """
        Create LLM provider based on config
        
        Args:
            provider_type: "anthropic" or "azure_openai"
                          If None, reads from LLM_PROVIDER env var
        
        Returns:
            The appropriate provider instance
        
        Example:
            # In .env file:
            # LLM_PROVIDER=anthropic
            
            provider = LLMProviderFactory.create()
            response = await provider.generate([...])
        """
        # Get provider type from env if not specified
        provider_type = provider_type or os.getenv("LLM_PROVIDER", "anthropic")
        
        if provider_type == "anthropic":
            # TODO: Get model from env
            model = os.getenv("ANTHROPIC_MODEL", "claude-sonnet-4-5")
            return AnthropicProvider(model=model)
        
        elif provider_type == "azure_openai":
            # TODO: Get deployment name from env
            deployment = os.getenv("AZURE_OPENAI_DEPLOYMENT_NAME", "gpt-4")
            return AzureOpenAIProvider(deployment_name=deployment)
        
        else:
            raise ValueError(f"Unknown provider type: {provider_type}")


# Simple wrapper for easy use
class LLMProvider:
    """
    Simple wrapper - automatically picks the right provider
    
    Usage:
        llm = LLMProvider()
        response = await llm.generate("What is Fair Work?")
    """
    
    def __init__(self):
        self.provider = LLMProviderFactory.create()
    
    async def generate(
        self, 
        messages: List[Dict[str, str]], 
        temperature: float = 0.7,
        max_tokens: int = 4096
    ) -> Dict[str, Any]:
        """
        Generate response using the configured provider
        """
        return await self.provider.generate(messages, temperature, max_tokens)