"""Base class for LangChain-backed LLM providers."""

from __future__ import annotations

import asyncio
from typing import Any, Dict, List

from langchain_core.messages import AIMessage, HumanMessage, SystemMessage

from .provider_base import LLMProviderBase


class LangChainProviderBase(LLMProviderBase):
    """Shared generate / count_tokens logic for all LangChain chat providers.

    Subclasses must set these attributes in __init__:
        self.chat            — a LangChain chat model (ChatOpenAI, ChatAnthropic, …)
        self.model           — model name string
        self.timeout_seconds — request timeout in seconds
    """

    @staticmethod
    def _to_langchain_messages(messages: List[Dict[str, str]]) -> list:
        lc_messages = []
        for message in messages:
            if not isinstance(message, dict):
                continue
            role = message.get("role", "user")
            content = message.get("content", "")
            if not isinstance(role, str) or not isinstance(content, str):
                continue
            if role == "system":
                lc_messages.append(SystemMessage(content=content))
            elif role == "assistant":
                lc_messages.append(AIMessage(content=content))
            else:
                lc_messages.append(HumanMessage(content=content))
        return lc_messages

    async def generate(
        self,
        messages: List[Dict[str, str]],
        temperature: float = 0.7,
        max_tokens: int = 4096,
    ) -> Dict[str, Any]:
        lc_messages = self._to_langchain_messages(messages)
        if not lc_messages:
            raise ValueError("No valid messages to send to LLM provider")
        try:
            response = await asyncio.wait_for(
                self.chat.ainvoke(
                    lc_messages, temperature=temperature, max_tokens=max_tokens
                ),
                timeout=self.timeout_seconds,
            )
        except asyncio.TimeoutError as exc:
            raise TimeoutError(
                f"LLM request timed out after {int(self.timeout_seconds)}s"
            ) from exc
        return {
            "content": response.content,
            "model": self.model,
            "usage": {},
        }

    def count_tokens(self, text: str) -> int:
        return len(text.split())
