"""
Pydantic models and enums for roster import.
"""

from __future__ import annotations

from typing import Any, Optional
from datetime import date, time, datetime, timedelta
from enum import Enum
from decimal import Decimal

from pydantic import BaseModel, ConfigDict, EmailStr, Field, computed_field
from pydantic.alias_generators import to_camel


class ParseIssueSeverity(Enum):
    """Severity level for parse issues."""
    ERROR = "error"
    WARNING = "warning"


class ParseResultStatus(Enum):
    """Overall parse result status."""
    OK = "ok"
    WARNING = "warning"
    ROW_ERROR = "row_error"
    BLOCKING = "blocking"


class ParseMode(Enum):
    """Parsing strictness mode."""
    STRICT = "strict"
    LENIENT = "lenient"


class ParseIssue(BaseModel):
    """Represents a parsing issue (error or warning)."""
    model_config = ConfigDict(
        alias_generator=to_camel,
        populate_by_name=True,
        use_enum_values=True,
    )

    severity: ParseIssueSeverity
    code: str
    message: str
    row: int
    column: Optional[str] = None
    value: Optional[str] = None
    hint: Optional[str] = None
    detail: Optional[str] = None


class ParseIssueError(Exception):
    """Exception wrapper for row-level parse issues."""

    def __init__(self, issue: ParseIssue):
        super().__init__(issue.message)
        self.issue = issue


class RosterEntry(BaseModel):
    """Represents a single roster entry (shift) parsed from Excel."""
    model_config = ConfigDict(
        alias_generator=to_camel,
        populate_by_name=True,
    )

    excel_row: int
    employee_email: Optional[EmailStr] = None
    employee_number: Optional[str] = None
    employee_name: Optional[str] = None
    employment_type: Optional[str] = None
    date: date
    start_time: time
    end_time: time
    is_overnight: bool = False
    has_meal_break: bool = False
    meal_break_duration: Optional[int] = None
    has_rest_breaks: bool = False
    rest_breaks_duration: Optional[int] = None
    is_public_holiday: bool = False
    public_holiday_name: Optional[str] = None
    is_on_call: bool = False
    location: Optional[str] = None
    notes: Optional[str] = None

    @computed_field
    @property
    def duration_hours(self) -> Decimal:
        """
        Shift duration in hours (gross, before breaks).
        Handles overnight shifts (e.g., 22:00 - 06:00 = 8 hours).
        """
        start_dt = datetime.combine(self.date, self.start_time)
        end_dt = datetime.combine(self.date, self.end_time)
        if self.is_overnight:
            end_dt += timedelta(days=1)
        duration = end_dt - start_dt
        return Decimal(str(duration.total_seconds() / 3600)).quantize(Decimal("0.01"))

    @computed_field
    @property
    def net_hours(self) -> Decimal:
        """Net working hours (duration minus breaks)."""
        total_break_minutes = (self.meal_break_duration or 0) + (self.rest_breaks_duration or 0)
        break_hours = Decimal(str(total_break_minutes / 60))
        return (self.duration_hours - break_hours).quantize(Decimal("0.01"))


class EmployeeEntry(BaseModel):
    """Represents a single employee entry parsed from Excel."""
    model_config = ConfigDict(
        alias_generator=to_camel,
        populate_by_name=True,
    )

    excel_row: int
    name: str
    email: Optional[EmailStr] = None
    role: str
    department: Optional[str] = None
    start_date: Optional[date] = None


class EmployeeParseResult(BaseModel):
    """Result of parsing an employee Excel file."""
    model_config = ConfigDict(
        alias_generator=to_camel,
        populate_by_name=True,
    )

    entries: list[EmployeeEntry]
    raw_rows: list[dict[str, Any]]


class RosterParseResult(BaseModel):
    """
    Result of parsing a roster Excel file.

    Contains parsed entries and summary statistics.
    """
    model_config = ConfigDict(
        alias_generator=to_camel,
        populate_by_name=True,
    )

    entries: list[RosterEntry]
    raw_rows: list[dict[str, Any]]

    @computed_field
    @property
    def week_start_date(self) -> Optional[date]:
        """Earliest shift date in the roster."""
        if not self.entries:
            return None
        return min(entry.date for entry in self.entries)

    @computed_field
    @property
    def week_end_date(self) -> Optional[date]:
        """Latest shift date in the roster."""
        if not self.entries:
            return None
        return max(entry.date for entry in self.entries)

    @computed_field
    @property
    def total_shifts(self) -> int:
        """Total number of shifts parsed."""
        return len(self.entries)

    @computed_field
    @property
    def total_hours(self) -> Decimal:
        """Total gross hours across all shifts."""
        if not self.entries:
            return Decimal("0.00")
        total = sum((entry.duration_hours for entry in self.entries), Decimal("0.00"))
        return total.quantize(Decimal("0.01"))

    @computed_field
    @property
    def unique_employees(self) -> int:
        """Number of unique employees in the roster."""
        if not self.entries:
            return 0
        keys: set[str] = set()
        for entry in self.entries:
            if entry.employee_number:
                keys.add(f"num:{entry.employee_number.strip().lower()}")
                continue
            if entry.employee_email:
                keys.add(f"email:{str(entry.employee_email).strip().lower()}")
                continue
            keys.add("unknown")
        return len(keys)


class IssueGroupSummary(BaseModel):
    """Aggregated summary for a group of issues."""
    model_config = ConfigDict(
        alias_generator=to_camel,
        populate_by_name=True,
    )

    code: str
    column: Optional[str] = None
    severity: ParseIssueSeverity
    count: int
    sample_rows: list[int] = Field(default_factory=list)
    sample_messages: list[str] = Field(default_factory=list)


class ParseResultSummary(BaseModel):
    """Summary of parsing outcome."""
    model_config = ConfigDict(
        alias_generator=to_camel,
        populate_by_name=True,
    )

    status: ParseResultStatus
    total_issues: int
    error_count: int
    warning_count: int
    blocking_count: int


class ParseResponse(BaseModel):
    """Top-level parse response wrapper."""
    model_config = ConfigDict(
        alias_generator=to_camel,
        populate_by_name=True,
        use_enum_values=True,
    )

    result: Any
    issues: list[ParseIssue]
    summary: ParseResultSummary
    issue_summary: list[IssueGroupSummary]
