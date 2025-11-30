import json

from fastapi import HTTPException, status
from pydantic import ValidationError

from llm import generate_reply
from agents.compliance.prompt_builder import COMPLIANCE_PROMPT
from agents.compliance.features.ask_ai_question.schemas import (
    AskAiQuestionRequest,
    AskAiQuestionResponse,
)

ASK_AI_RESPONSE_FORMAT = {
    "name": "AskAiQuestionResponse",
    "schema": AskAiQuestionResponse.model_json_schema(),
}

def _llm_snippet(text: str, limit: int = 400) -> str:
    text = text.strip()
    if len(text) <= limit:
        return text
    return f"{text[:limit]}... (truncated)"


def run(req: AskAiQuestionRequest) -> AskAiQuestionResponse:
    reply = generate_reply(
        COMPLIANCE_PROMPT,
        req.question,
        response_schema=ASK_AI_RESPONSE_FORMAT,
    )
    if not reply:
        raise HTTPException(
            status_code=status.HTTP_502_BAD_GATEWAY,
            detail="LLM returned an empty response.",
        )

    try:
        payload = json.loads(reply)
    except json.JSONDecodeError as exc:
        raise HTTPException(
            status_code=status.HTTP_502_BAD_GATEWAY,
            detail={
                "message": "LLM returned invalid JSON.",
                "llm_output": _llm_snippet(reply),
            },
        ) from exc

    try:
        return AskAiQuestionResponse(**payload)
    except ValidationError as exc:
        raise HTTPException(
            status_code=status.HTTP_502_BAD_GATEWAY,
            detail={
                "message": "LLM response did not match the expected schema.",
                "errors": exc.errors(),
                "llm_output": _llm_snippet(reply),
            },
        ) from exc
