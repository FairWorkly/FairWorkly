from pathlib import Path

from fastapi import APIRouter

from agents.compliance.features.ask_ai_question.schemas import (
    AskAiQuestionRequest,
    AskAiQuestionResponse,
)
from agents.compliance.features.ask_ai_question.handler import run as handle_ask_ai_question

router = APIRouter(prefix="/agents/compliance", tags=["Compliance Agent"])


@router.post("/qa", response_model=AskAiQuestionResponse)
async def ask_compliance(req: AskAiQuestionRequest) -> AskAiQuestionResponse:
    return handle_ask_ai_question(req)
