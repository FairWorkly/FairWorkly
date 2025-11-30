from pydantic import BaseModel, HttpUrl
from typing import List, Literal

RiskLevel = Literal["green", "yellow", "red"]

class ComplianceReference(BaseModel):
    source: Literal["NES", "Award", "FairWork", "Other"]
    title: str
    url: HttpUrl | None = None
    note: str | None = None

class AskAiQuestionRequest(BaseModel):
    question: str
    org_id: str | None = None
    user_id: str | None = None
    award_code: str | None = None
    audience: Literal["manager", "employee"] = "manager"

    precomputed_findings: dict | None = None

class AskAiQuestionResponse(BaseModel):
    plain_explanation: str
    key_points: List[str]
    risk_level: RiskLevel
    what_to_do_next: List[str]
    legal_references: List[ComplianceReference]
    disclaimer: str
