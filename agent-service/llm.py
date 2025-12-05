import logging
import os
from functools import lru_cache
from pathlib import Path

import yaml
from dotenv import load_dotenv
from langchain_core.messages import HumanMessage, SystemMessage
from langchain_openai import ChatOpenAI

load_dotenv()

logger = logging.getLogger(__name__)

DEFAULT_MODEL = "gpt-4o-mini"
DEFAULT_TEMPERATURE = 0.0
CONFIG_PATH = Path(os.getenv("CONFIG_PATH", "config.yaml"))


def _resolve_model_settings() -> tuple[str, float]:
    """config reader: expect config.yaml and model block to exist."""
    config = yaml.safe_load(CONFIG_PATH.read_text(encoding="utf-8"))
    model_block = config.get("model", {})

    name = model_block.get("name", DEFAULT_MODEL)
    temperature = float(model_block.get("temperature", DEFAULT_TEMPERATURE))
    return name, temperature


@lru_cache(maxsize=1)
def _get_llm() -> ChatOpenAI:
    """Lazy-initialize the LLM client (cached for reuse)."""
    model, temperature = _resolve_model_settings()
    return ChatOpenAI(model=model, temperature=temperature)


@lru_cache(maxsize=None)
def load_prompt(prompt_path: Path | str, fallback: str) -> str:
    """Read a prompt file once and fall back to a default when missing."""
    path = Path(prompt_path)
    try:
        return path.read_text(encoding="utf-8").strip()
    except FileNotFoundError:
        logger.warning("Prompt file %s not found; using fallback content", path)
        return fallback


class LLMInvocationError(Exception):
    """Raised when the LLM backend fails."""


def generate_reply(
    system_prompt: str,
    message: str,
    response_format: dict | None = None,
) -> str:
    messages = [
        SystemMessage(content=system_prompt),
        HumanMessage(content=message),
    ]
    kwargs = {}
    if response_format:
        kwargs["response_format"] = response_format
    try:
        response = _get_llm().invoke(messages, **kwargs)
        return response.content
    except Exception as exc:
        logger.exception("LLM invocation failed")
        raise LLMInvocationError("LLM invocation failed") from exc
