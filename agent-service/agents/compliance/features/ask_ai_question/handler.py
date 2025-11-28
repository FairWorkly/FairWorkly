from pathlib import Path

from llm import generate_reply, load_prompt
from agents.compliance.features.ask_ai_question.schemas import (
    AskAiQuestionRequest,
    AskAiQuestionResponse,
)

PROMPT = load_prompt(
    Path(__file__).resolve().parents[1] / "system_prompt.txt",
    "You are FairWorkly's compliance agent. Follow Australian HR rules.",
)


def run(req: AskAiQuestionRequest) -> AskAiQuestionResponse:
    reply = generate_reply(PROMPT, req.question)
    return AskAiQuestionResponse(reply=reply)
