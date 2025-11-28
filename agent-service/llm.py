from dotenv import load_dotenv
import os
from pathlib import Path
from functools import lru_cache

load_dotenv()

from langchain_openai import ChatOpenAI
from langchain_core.messages import SystemMessage, HumanMessage

llm = ChatOpenAI(
    model=os.getenv("OPENAI_MODEL", "gpt-4o-mini"),
    temperature=float(os.getenv("MODEL_TEMPERATURE", "0")),
)


@lru_cache(maxsize=None)
def load_prompt(prompt_path: Path, fallback: str) -> str:
    try:
        return Path(prompt_path).read_text().strip()
    except FileNotFoundError:
        return fallback


def generate_reply(system_prompt: str, message: str) -> str:
    messages = [
        SystemMessage(content=system_prompt),
        HumanMessage(content=message),
    ]
    try:
        response = llm.invoke(messages)
        return response.content
    except Exception:
        return "Sorry, I'm unable to respond right now. Please try again later."
