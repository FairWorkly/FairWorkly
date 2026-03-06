"""Pydantic request models for the Payroll Explain endpoint."""

from typing import Optional

from pydantic import BaseModel


class IssueDescription(BaseModel):
    actualValue: float
    expectedValue: float
    affectedUnits: float
    unitType: str
    contextLabel: str


class PayrollExplainRequest(BaseModel):
    issueId: str
    categoryType: str
    employeeName: str
    employeeId: str
    severity: str
    impactAmount: float
    description: IssueDescription
    warning: Optional[str] = None
