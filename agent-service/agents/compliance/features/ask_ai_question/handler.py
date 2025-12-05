import asyncio
import logging
from typing import List

from fastapi import HTTPException, status
from langchain_core.documents import Document
from pydantic import ValidationError

from shared.model import generate_reply
from shared.errors import LLMResponseError
from shared.document_processor import DocumentProcessor
from shared.query_handler import QueryHandler
from agents.compliance.prompt_builder import COMPLIANCE_PROMPT
from agents.compliance.features.ask_ai_question.schemas import (
    AskAiQuestionRequest,
    AskAiQuestionResponse,
)
from agents.compliance.rag import PROMPT_PATH

RESP_FORMAT = {
    "type": "json_schema",
    "json_schema": {
        "name": "AskAiQuestionResponse",
        "schema": AskAiQuestionResponse.model_json_schema(),
    },
}

logger = logging.getLogger(__name__)
_query_handler: QueryHandler | None = None


def _get_query_handler() -> QueryHandler:
    global _query_handler
    if _query_handler is None:
        _query_handler = QueryHandler(
            DocumentProcessor(prompt_file=str(PROMPT_PATH))
        )
    return _query_handler


def _augment_question(question: str, docs: List[Document], char_limit: int = 2000) -> str:
    context_parts = []
    remaining = char_limit
    for doc in docs:
        snippet = doc.page_content.strip()
        if not snippet:
            continue
        if len(snippet) > remaining:
            snippet = snippet[:remaining]
        context_parts.append(snippet)
        remaining -= len(snippet)
        if remaining <= 0:
            break
    if not context_parts:
        return question
    context_text = "\n\n".join(context_parts)
    return f"{question}\n\nContext:\n{context_text}"


def _handle_request(req: AskAiQuestionRequest) -> AskAiQuestionResponse:
    try:
        docs = _get_query_handler().retrieve_documents(req.question)
    except Exception as exc:
        logger.warning("Context retrieval failed for question_id=%s: %s", req.question_id, exc)
        docs = []

    question = _augment_question(req.question, docs)

    try:
        reply = generate_reply(
            COMPLIANCE_PROMPT,
            question,
            response_format=RESP_FORMAT
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
