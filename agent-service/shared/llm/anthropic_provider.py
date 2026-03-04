"""Anthropic LLM provider backed by LangChain ChatAnthropic."""

from __future__ import annotations

import os

from langchain_anthropic import ChatAnthropic

from .langchain_base import LangChainProviderBase


class AnthropicProvider(LangChainProviderBase):
    """Anthropic provider using LangChain ChatAnthropic."""

    def __init__(
        self,
        model: str | None = None,
        *,
        api_key: str | None = None,
        temperature: float | None = None,
    ):
        self.model = model or os.getenv("ANTHROPIC_MODEL", "claude-sonnet-4-20250514")
        self.api_key = api_key or os.getenv("ANTHROPIC_API_KEY")
        self.timeout_seconds = float(os.getenv("LLM_TIMEOUT_SECONDS", "120"))
        if not self.api_key:
            raise ValueError("ANTHROPIC_API_KEY must be set")

        self.chat = ChatAnthropic(
            model=self.model,
            temperature=temperature if temperature is not None else 0.3,
            anthropic_api_key=self.api_key,
        )
