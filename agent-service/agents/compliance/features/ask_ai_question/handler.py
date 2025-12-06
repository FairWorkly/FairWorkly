import asyncio

from fastapi import HTTPException, status
from pydantic import ValidationError

from errors import LLMResponseError
from llm import generate_reply
from agents.compliance.prompt_builder import COMPLIANCE_PROMPT
from agents.compliance.features.ask_ai_question.schemas import (
    AskAiQuestionRequest,
    AskAiQuestionResponse,
)

RESP_FORMAT = {
    "type": "json_schema",
    "json_schema": {
        "name": "AskAiQuestionResponse",
        "schema": AskAiQuestionResponse.model_json_schema(),
    },
}


def _handle_request(req: AskAiQuestionRequest) -> AskAiQuestionResponse:
    try:
        reply = generate_reply(
            COMPLIANCE_PROMPT,
            req.question,
            response_format=RESP_FORMAT,
        )
    except LLMResponseError as exc:
        raise HTTPException(
            status_code=status.HTTP_502_BAD_GATEWAY,
            detail="LLM request failed.",
        ) from exc
    if not reply:
        raise HTTPException(
            status_code=status.HTTP_502_BAD_GATEWAY,
            detail="LLM returned an empty response.",
        )

    try:
        return AskAiQuestionResponse.model_validate_json(reply)
    except ValidationError as exc:
        raise HTTPException(
            status_code=status.HTTP_502_BAD_GATEWAY,
            detail={
                "message": "LLM returned invalid JSON or an unexpected schema.",
                "errors": exc.errors(),
                "llm_output": reply,
            },
        ) from exc


async def handle_ask_ai_question(req: AskAiQuestionRequest) -> AskAiQuestionResponse:
    return await asyncio.to_thread(_handle_request, req)
