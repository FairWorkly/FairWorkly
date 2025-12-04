import logging

from fastapi import APIRouter

from agents.compliance.features.ask_ai_question.schemas import (
    AskAiQuestionRequest,
    AskAiQuestionResponse,
)
from agents.compliance.features.ask_ai_question.handler import (
    handle_ask_ai_question,
)

logger = logging.getLogger(__name__)

router = APIRouter(prefix="/agents/compliance", tags=["Compliance"])


@router.post("/qa", response_model=AskAiQuestionResponse)
async def ask_compliance(req: AskAiQuestionRequest) -> AskAiQuestionResponse:
    logger.info(
        "Compliance QA request question_id=%s audience=%s award_code=%s",
        req.question_id,
        req.audience,
        req.award_code or "-",
    )
    return await handle_ask_ai_question(req)
