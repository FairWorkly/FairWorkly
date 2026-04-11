"""Pydantic models for the multi-agent debate feature."""

from __future__ import annotations

from typing import Any, Optional

from pydantic import BaseModel, Field


class ShiftScenario(BaseModel):
    """Input scenario describing a shift to be evaluated."""

    employee_name: str = Field(..., description="Employee name, e.g. 'Alice'")
    shift_date: str = Field(..., description="Date of the shift, e.g. '2024-03-16 (Saturday)'")
    shift_hours: float = Field(..., description="Total hours worked on this shift")
    week_hours_before_shift: float = Field(
        0,
        description="Hours already worked earlier in the week (Mon-Fri) before this shift",
    )
    award_name: str = Field(
        "Hospitality Industry (General) Award 2020",
        description="Applicable Modern Award name",
    )
    extra_context: Optional[str] = Field(
        None,
        description="Any additional context (e.g. public holiday, casual employee, etc.)",
    )


class DebateRequest(BaseModel):
    """API request body for the debate endpoint."""

    scenario: ShiftScenario


class DebateRound(BaseModel):
    """One round of the multi-agent debate."""

    agent: str = Field(..., description="Agent name: Roster Agent / Payroll Agent / Fair Bot")
    role: str = Field(..., description="Agent role label")
    icon: str = Field(..., description="Display icon for this agent")
    stance: str = Field(..., description="One-line position summary")
    reasoning: str = Field(..., description="Full reasoning text from the agent")
    challenges: Optional[str] = Field(
        None,
        description="What this agent disagrees with from previous rounds",
    )
    sources: list[dict[str, Any]] = Field(
        default_factory=list,
        description="Award sources cited (from RAG retrieval)",
    )


class DebateResult(BaseModel):
    """Complete debate output returned to the client."""

    scenario_summary: str = Field(..., description="Plain-language restatement of the scenario")
    rounds: list[DebateRound]
    final_ruling: str = Field(..., description="The arbitrated outcome")
    cited_award_section: Optional[str] = Field(
        None, description="Key Award section cited in the ruling"
    )
    model: Optional[str] = Field(None, description="LLM model used")
