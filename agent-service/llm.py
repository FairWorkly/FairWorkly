import os
from functools import lru_cache
from pathlib import Path

import yaml
from dotenv import load_dotenv
from langchain_core.messages import HumanMessage, SystemMessage
from langchain_openai import ChatOpenAI

from errors import LLMResponseError

load_dotenv()

DEFAULT_MODEL = "gpt-4o-mini"
DEFAULT_TEMPERATURE = 0.0
CONFIG_PATH = Path(os.getenv("CONFIG_PATH", "config.yaml"))


def _resolve_model_settings() -> tuple[str, float]:
    config = yaml.safe_load(CONFIG_PATH.read_text(encoding="utf-8"))
    model_block = config.get("model", {})

    name = model_block.get("name", DEFAULT_MODEL)
    temperature = float(model_block.get("temperature", DEFAULT_TEMPERATURE))
    return name, temperature


@lru_cache(maxsize=1)
def _get_llm() -> ChatOpenAI:
    model, temperature = _resolve_model_settings()
    return ChatOpenAI(model=model, temperature=temperature)


@lru_cache(maxsize=None)
def load_prompt(prompt_path: Path | str, fallback: str) -> str:
    path = Path(prompt_path)
    try:
        return path.read_text(encoding="utf-8").strip()
    except FileNotFoundError:
        return fallback


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
        raise LLMResponseError("LLM invocation failed") from exc
