import asyncio
import json
import logging

from fastapi import HTTPException, status
from pydantic import ValidationError

from llm import LLMInvocationError, generate_reply
from agents.compliance.prompt_builder import COMPLIANCE_PROMPT
from agents.compliance.features.ask_ai_question.schemas import (
    AskAiQuestionRequest,
    AskAiQuestionResponse,
)

logger = logging.getLogger(__name__)


ASK_AI_RESPONSE_FORMAT = {
    "name": "AskAiQuestionResponse",
    "schema": AskAiQuestionResponse.model_json_schema(),
}

def _llm_snippet(text: str, limit: int = 400) -> str:
    text = text.strip()
    if len(text) <= limit:
        return text
    return f"{text[:limit]}... (truncated)"


def _handle_request(req: AskAiQuestionRequest) -> AskAiQuestionResponse:
    try:
        reply = generate_reply(
            COMPLIANCE_PROMPT,
            req.question,
            response_schema=ASK_AI_RESPONSE_FORMAT,
        )
    except LLMInvocationError as exc:
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


async def handle_ask_ai_question(req: AskAiQuestionRequest) -> AskAiQuestionResponse:
    """Async interface for the compliance Q&A handler."""
    logger.debug(
        "Handling compliance QA request question_id=%s audience=%s award_code=%s",
        req.question_id,
        req.audience,
        req.award_code or "-",
    )
    return await asyncio.to_thread(_handle_request, req)
