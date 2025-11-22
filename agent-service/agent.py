from dotenv import load_dotenv
load_dotenv() 
from langchain_openai import ChatOpenAI
from langchain_core.messages import SystemMessage, HumanMessage

llm = ChatOpenAI(
    model="gpt-4o-mini",
    temperature=0,
)

SYSTEM_PROMPT = "You are a helpful Compliance assistant."


def generate_reply(message: str) -> str:
    """
    Generate a reply from the Compliance assistant given a single user message.
    """
    messages = [
        SystemMessage(content=SYSTEM_PROMPT),
        HumanMessage(content=message),
    ]
    response = llm.invoke(messages)
    return response.content