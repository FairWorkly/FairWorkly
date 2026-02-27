"""OpenAI LLM provider backed by LangChain ChatOpenAI."""

from __future__ import annotations

import os

from langchain_openai import ChatOpenAI

from .langchain_base import LangChainProviderBase


class OpenAIProvider(LangChainProviderBase):
    """OpenAI provider using LangChain ChatOpenAI."""

    def __init__(
        self,
        model: str | None = None,
        *,
        api_key: str | None = None,
        api_base: str | None = None,
        temperature: float | None = None,
    ):
        self.model = model or os.getenv("OPENAI_MODEL", "gpt-4o-mini")
        self.api_key = api_key or os.getenv("OPENAI_API_KEY")
        self.api_base = api_base or os.getenv("OPENAI_API_BASE")
        self.timeout_seconds = float(os.getenv("LLM_TIMEOUT_SECONDS", "120"))
        if not self.api_key:
            raise ValueError("OPENAI_API_KEY must be set for OpenAI provider")

        self.chat = ChatOpenAI(
            model=self.model,
            temperature=temperature or float(os.getenv("OPENAI_TEMPERATURE", 0.3)),
            openai_api_key=self.api_key,
            openai_api_base=self.api_base,
        )
