"""Local HuggingFace-based LLM provider."""

from __future__ import annotations

import asyncio
from typing import List, Dict, Any

from langchain_huggingface import HuggingFacePipeline
from transformers import AutoModelForCausalLM, AutoTokenizer, pipeline

from .provider_base import LLMProviderBase


class LocalHuggingFaceProvider(LLMProviderBase):
    """Runs a local HuggingFace pipeline for text generation."""

    def __init__(self, config: Dict[str, Any]):
        model_params = config.get("model_params", {})
        self.model_id = model_params.get("local_model_id")
        if not self.model_id:
            raise ValueError("local_model_id must be configured for local LLM mode")

        task = model_params.get("local_model_task_type", "text-generation")
        max_new_tokens = model_params.get("local_max_tokens", 512)
        top_k = model_params.get("local_top_k", 50)
        top_p = model_params.get("local_top_p", 0.95)
        temperature = model_params.get("temperature", 0.3)
        repetition_penalty = model_params.get("local_repetition_penalty", 1.1)

        tokenizer = AutoTokenizer.from_pretrained(self.model_id)
        model = AutoModelForCausalLM.from_pretrained(self.model_id)
        generation_pipeline = pipeline(
            task,
            model=model,
            tokenizer=tokenizer,
            max_new_tokens=max_new_tokens,
            top_k=top_k,
            top_p=top_p,
            temperature=temperature,
            repetition_penalty=repetition_penalty,
            do_sample=True,
        )
        self.chat = HuggingFacePipeline(pipeline=generation_pipeline)

    def _format_messages(self, messages: List[Dict[str, str]]) -> str:
        parts = []
        for message in messages:
            role = message.get("role", "user").capitalize()
            content = message.get("content", "")
            parts.append(f"{role}: {content}")
        parts.append("Assistant:")
        return "\n".join(parts)

    async def generate(
        self,
        messages: List[Dict[str, str]],
        temperature: float = 0.7,
        max_tokens: int = 4096,
    ) -> Dict[str, Any]:
        prompt = self._format_messages(messages)

        def call_model() -> str:
            return self.chat.invoke(prompt)

        response = await asyncio.to_thread(call_model)
        content = response if isinstance(response, str) else str(response)
        return {
            "content": content,
            "model": self.model_id,
            "usage": {"prompt_tokens": self.count_tokens(prompt)},
        }

    def count_tokens(self, text: str) -> int:
        return len(text.split())
