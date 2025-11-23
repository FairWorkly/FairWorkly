from dotenv import load_dotenv
import os

load_dotenv()

from langchain_openai import ChatOpenAI
from langchain_core.messages import SystemMessage, HumanMessage

llm = ChatOpenAI(
    model=os.getenv("OPENAI_MODEL", "gpt-4o-mini"),
    temperature=float(os.getenv("MODEL_TEMPERATURE", "0")),
)

SYSTEM_PROMPT = open("system_prompt.txt").read().strip()


def generate_reply(message: str) -> str:
    messages = [
        SystemMessage(content=SYSTEM_PROMPT),
        HumanMessage(content=message),
    ]
    try:
        response = llm.invoke(messages)
        return response.content
    except Exception:
        return "Sorry, I'm unable to respond right now. Please try again later."