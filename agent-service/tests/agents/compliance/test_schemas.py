import pytest
from pydantic import ValidationError

from agents.compliance.features.ask_ai_question.schemas import (
    AskAiQuestionRequest,
    AskAiQuestionResponse,
    ComplianceReference,
)


def _base_reference() -> ComplianceReference:
    return ComplianceReference(
        source="NES",
        title="NES overview",
        url="https://www.fairwork.gov.au",
    )


def test_request_requires_question() -> None:
    with pytest.raises(ValidationError):
        AskAiQuestionRequest(question_id="q-1")


def test_request_requires_question_id() -> None:
    with pytest.raises(ValidationError):
        AskAiQuestionRequest(question="What about overtime?")


def test_request_rejects_invalid_audience() -> None:
    with pytest.raises(ValidationError):
        AskAiQuestionRequest(
            question_id="q-1",
            question="What are our overtime obligations?",
            audience="supervisor",
        )


def test_response_rejects_invalid_risk_level() -> None:
    with pytest.raises(ValidationError):
        AskAiQuestionResponse(
            question_id="q-1",
            plain_explanation="Explanation",
            key_points=["Point"],
            risk_level="blue",
            what_to_do_next=["Action"],
            legal_references=[_base_reference()],
            disclaimer="Disclaimer",
        )


def test_reference_rejects_bad_url() -> None:
    with pytest.raises(ValidationError):
        ComplianceReference(
            source="NES",
            title="NES overview",
            url="not-a-url",
        )
