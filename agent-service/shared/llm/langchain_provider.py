"""LangChain-backed provider using ChatOpenAI."""

from __future__ import annotations

import os
from typing import List, Dict, Any

from langchain_core.messages import SystemMessage, HumanMessage, AIMessage
from langchain_openai import ChatOpenAI

from .provider_base import LLMProviderBase


class LangChainOpenAIProvider(LLMProviderBase):
    """LLM provider that relies on LangChain ChatOpenAI."""

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
        if not self.api_key:
            raise ValueError("OPENAI_API_KEY must be set for LangChain provider")

        self.chat = ChatOpenAI(
            model=self.model,
            temperature=temperature or float(os.getenv("OPENAI_TEMPERATURE", 0.3)),
            openai_api_key=self.api_key,
            openai_api_base=self.api_base,
        )

    async def generate(
        self,
        messages: List[Dict[str, str]],
        temperature: float = 0.7,
        max_tokens: int = 4096,
    ) -> Dict[str, Any]:
        lc_messages = []
        for message in messages:
            role = message.get("role", "user")
            content = message.get("content", "")
            if role == "system":
                lc_messages.append(SystemMessage(content=content))
            elif role == "assistant":
                lc_messages.append(AIMessage(content=content))
            else:
                lc_messages.append(HumanMessage(content=content))

        response = await self.chat.ainvoke(
            lc_messages, temperature=temperature, max_tokens=max_tokens
        )
        return {
            "content": response.content,
            "model": self.model,
            "usage": {},
        }

    def count_tokens(self, text: str) -> int:
        return len(text.split())
