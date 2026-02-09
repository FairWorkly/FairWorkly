"""Row-level parsers for roster and employee sheets."""

from __future__ import annotations

from typing import Any
from datetime import datetime, timedelta

from pydantic import EmailStr, TypeAdapter, ValidationError

from .header_map import HeaderMap
from .models import (
    EmployeeEntry,
    ParseIssue,
    ParseIssueError,
    ParseIssueSeverity,
    ParseMode,
    RosterEntry,
)
from .utils import (
    get_string,
    normalize_employment_type,
    parse_boolean,
    parse_date,
    parse_int,
    parse_time,
)

_email_validator = TypeAdapter(EmailStr)


def parse_roster_row(
    row: dict[str, Any],
    row_num: int,
    header_map: HeaderMap,
    mode: ParseMode,
) -> tuple[RosterEntry, list[ParseIssue]]:
    """Parse a single row into a RosterEntry with warnings."""
    warnings: list[ParseIssue] = []

    # Optimize: normalize and detect duplicates in single pass
    normalized, duplicate_headers = header_map.normalize_and_detect_duplicates(row)
    for canonical, headers in duplicate_headers.items():
        issue = ParseIssue(
            row=row_num,
            severity=ParseIssueSeverity.WARNING,
            code="DUPLICATE_CANONICAL_COLUMN_IN_ROW",
            message=f"Multiple columns map to '{canonical}': {', '.join(headers)}",
            column=canonical,
        )
        if mode == ParseMode.STRICT:
            issue.severity = ParseIssueSeverity.ERROR
            raise ParseIssueError(issue)
        warnings.append(issue)

    employee_number = get_string(normalized.get("employee_number"))
    if not employee_number:
        raise ParseIssueError(
            ParseIssue(
                row=row_num,
                severity=ParseIssueSeverity.ERROR,
                code="MISSING_REQUIRED_FIELD",
                message="Employee Number is required for roster import (email is optional)",
                column="employee_number",
            )
        )

    raw_email = normalized.get("employee_email")
    employee_email = get_string(raw_email)
    if employee_email:
        try:
            _email_validator.validate_python(employee_email)
        except ValidationError as exc:
            raise ParseIssueError(
                ParseIssue(
                    row=row_num,
                    severity=ParseIssueSeverity.ERROR,
                    code="INVALID_EMAIL",
                    message=f"Invalid email format: {employee_email}",
                    column="employee_email",
                    value=employee_email,
                )
            ) from exc

    raw_date = normalized.get("date")
    try:
        date_value = parse_date(raw_date)
    except ValueError as exc:
        raise ParseIssueError(
            ParseIssue(
                row=row_num,
                severity=ParseIssueSeverity.ERROR,
                code="INVALID_DATE",
                message=str(exc),
                column="date",
                value=get_string(raw_date),
                hint="Expected formats: YYYY-MM-DD, DD/MM/YYYY, MM/DD/YYYY, or Excel date number",
            )
        ) from exc
    if not date_value:
        raise ParseIssueError(
            ParseIssue(
                row=row_num,
                severity=ParseIssueSeverity.ERROR,
                code="MISSING_REQUIRED_FIELD",
                message="Date is required",
                column="date",
            )
        )

    raw_start = normalized.get("start_time")
    try:
        start_time = parse_time(raw_start)
    except ValueError as exc:
        raise ParseIssueError(
            ParseIssue(
                row=row_num,
                severity=ParseIssueSeverity.ERROR,
                code="INVALID_TIME",
                message=str(exc),
                column="start_time",
                value=get_string(raw_start),
                hint="Expected formats: HH:MM (24-hour), H:MM AM/PM, or Excel time number",
            )
        ) from exc
    if not start_time:
        raise ParseIssueError(
            ParseIssue(
                row=row_num,
                severity=ParseIssueSeverity.ERROR,
                code="MISSING_REQUIRED_FIELD",
                message="Start Time is required",
                column="start_time",
            )
        )

    raw_end = normalized.get("end_time")
    try:
        end_time = parse_time(raw_end)
    except ValueError as exc:
        raise ParseIssueError(
            ParseIssue(
                row=row_num,
                severity=ParseIssueSeverity.ERROR,
                code="INVALID_TIME",
                message=str(exc),
                column="end_time",
                value=get_string(raw_end),
                hint="Expected formats: HH:MM (24-hour), H:MM AM/PM, or Excel time number",
            )
        ) from exc
    if not end_time:
        raise ParseIssueError(
            ParseIssue(
                row=row_num,
                severity=ParseIssueSeverity.ERROR,
                code="MISSING_REQUIRED_FIELD",
                message="End Time is required",
                column="end_time",
            )
        )

    raw_overnight = normalized.get("is_overnight")
    explicit_provided = raw_overnight is not None and str(raw_overnight).strip() != ""
    explicit_value = parse_boolean(raw_overnight) if explicit_provided else None
    inferred_overnight = end_time < start_time
    is_overnight = explicit_value if explicit_provided else inferred_overnight

    if inferred_overnight and not explicit_provided:
        issue = ParseIssue(
            row=row_num,
            severity=ParseIssueSeverity.WARNING,
            code="OVERNIGHT_ASSUMED",
            message="end_time is earlier than start_time; overnight assumed",
            column="end_time",
            value=f"{start_time.isoformat()}-{end_time.isoformat()}",
            hint="Confirm end time or set 'Is Overnight' explicitly.",
        )
        if mode == ParseMode.STRICT:
            issue.severity = ParseIssueSeverity.ERROR
            raise ParseIssueError(issue)
        warnings.append(issue)

    has_meal_break = parse_boolean(normalized.get("has_meal_break"))
    meal_break_duration, meal_warning = parse_int(normalized.get("meal_break_duration"))
    if meal_warning:
        warnings.append(
            ParseIssue(
                row=row_num,
                severity=ParseIssueSeverity.WARNING,
                code="FRACTIONAL_VALUE_ROUNDED",
                message=meal_warning,
                column="meal_break_duration",
                value=str(normalized.get("meal_break_duration")),
                hint="Duration should be a whole number in minutes",
            )
        )
    if meal_break_duration is not None and meal_break_duration < 0:
        warnings.append(
            ParseIssue(
                row=row_num,
                severity=ParseIssueSeverity.WARNING,
                code="INVALID_DURATION_NEGATIVE",
                message=f"Meal break duration cannot be negative: {meal_break_duration}",
                column="meal_break_duration",
                value=str(meal_break_duration),
                hint="Duration must be a positive number in minutes",
            )
        )
        meal_break_duration = None
    if has_meal_break and meal_break_duration is None:
        warnings.append(
            ParseIssue(
                row=row_num,
                severity=ParseIssueSeverity.WARNING,
                code="MEAL_BREAK_DURATION_MISSING",
                message="has_meal_break=true but meal_break_duration is missing",
                column="meal_break_duration",
            )
        )

    has_rest_breaks = parse_boolean(normalized.get("has_rest_breaks"))
    rest_breaks_duration, rest_warning = parse_int(normalized.get("rest_breaks_duration"))
    if rest_warning:
        warnings.append(
            ParseIssue(
                row=row_num,
                severity=ParseIssueSeverity.WARNING,
                code="FRACTIONAL_VALUE_ROUNDED",
                message=rest_warning,
                column="rest_breaks_duration",
                value=str(normalized.get("rest_breaks_duration")),
                hint="Duration should be a whole number in minutes",
            )
        )
    if rest_breaks_duration is not None and rest_breaks_duration < 0:
        warnings.append(
            ParseIssue(
                row=row_num,
                severity=ParseIssueSeverity.WARNING,
                code="INVALID_DURATION_NEGATIVE",
                message=f"Rest breaks duration cannot be negative: {rest_breaks_duration}",
                column="rest_breaks_duration",
                value=str(rest_breaks_duration),
                hint="Duration must be a positive number in minutes",
            )
        )
        rest_breaks_duration = None
    if has_rest_breaks and rest_breaks_duration is None:
        warnings.append(
            ParseIssue(
                row=row_num,
                severity=ParseIssueSeverity.WARNING,
                code="REST_BREAKS_DURATION_MISSING",
                message="has_rest_breaks=true but rest_breaks_duration is missing",
                column="rest_breaks_duration",
            )
        )

    total_break_minutes = (meal_break_duration or 0) + (rest_breaks_duration or 0)
    if total_break_minutes > 0:
        start_dt = datetime.combine(date_value, start_time)
        end_dt = datetime.combine(date_value, end_time)
        if is_overnight:
            end_dt += timedelta(days=1)
        duration_minutes = int(round((end_dt - start_dt).total_seconds() / 60))

        if duration_minutes > 0 and total_break_minutes > duration_minutes:
            column = "meal_break_duration" if meal_break_duration else "rest_breaks_duration"
            warnings.append(
                ParseIssue(
                    row=row_num,
                    severity=ParseIssueSeverity.WARNING,
                    code="BREAK_EXCEEDS_SHIFT_DURATION",
                    message=(
                        f"Total break minutes ({total_break_minutes}) exceed shift duration minutes ({duration_minutes}). "
                        "This will distort net-hours calculations."
                    ),
                    column=column,
                    value=str(total_break_minutes),
                    hint="Fix break durations or shift times so breaks do not exceed the shift length.",
                )
            )

    raw_employment_type = get_string(normalized.get("employment_type"))
    employment_type = normalize_employment_type(raw_employment_type)
    if raw_employment_type is None:
        issue = ParseIssue(
            row=row_num,
            severity=ParseIssueSeverity.WARNING,
            code="EMPLOYMENT_TYPE_MISSING",
            message="employment_type is missing",
            column="employment_type",
            hint="Use full-time, part-time, or casual.",
        )
        if mode == ParseMode.STRICT:
            issue.severity = ParseIssueSeverity.ERROR
            raise ParseIssueError(issue)
        warnings.append(issue)
    elif employment_type is None:
        issue = ParseIssue(
            row=row_num,
            severity=ParseIssueSeverity.WARNING,
            code="EMPLOYMENT_TYPE_UNRECOGNIZED",
            message=f"employment_type is unrecognized: {raw_employment_type}",
            column="employment_type",
            value=raw_employment_type,
            hint="Use full-time, part-time, or casual.",
        )
        if mode == ParseMode.STRICT:
            issue.severity = ParseIssueSeverity.ERROR
            raise ParseIssueError(issue)
        warnings.append(issue)

    entry = RosterEntry(
        excel_row=row_num,
        employee_email=employee_email,
    employee_number=employee_number,
        employee_name=get_string(normalized.get("employee_name")),
        employment_type=employment_type or raw_employment_type,
        date=date_value,
        start_time=start_time,
        end_time=end_time,
        is_overnight=is_overnight,
        has_meal_break=has_meal_break,
        meal_break_duration=meal_break_duration,
        has_rest_breaks=has_rest_breaks,
        rest_breaks_duration=rest_breaks_duration,
        is_public_holiday=parse_boolean(normalized.get("is_public_holiday")),
        public_holiday_name=get_string(normalized.get("public_holiday_name")),
        is_on_call=parse_boolean(normalized.get("is_on_call")),
        location=get_string(normalized.get("location")),
        notes=get_string(normalized.get("notes")),
    )

    return entry, warnings


def parse_employee_row(
    row: dict[str, Any],
    row_num: int,
    header_map: HeaderMap,
    mode: ParseMode,
) -> tuple[EmployeeEntry, list[ParseIssue]]:
    """Parse a single row into an EmployeeEntry with warnings."""
    warnings: list[ParseIssue] = []

    # Optimize: normalize and detect duplicates in single pass
    normalized, duplicate_headers = header_map.normalize_and_detect_duplicates(row)
    for canonical, headers in duplicate_headers.items():
        issue = ParseIssue(
            row=row_num,
            severity=ParseIssueSeverity.WARNING,
            code="DUPLICATE_CANONICAL_COLUMN_IN_ROW",
            message=f"Multiple columns map to '{canonical}': {', '.join(headers)}",
            column=canonical,
        )
        if mode == ParseMode.STRICT:
            issue.severity = ParseIssueSeverity.ERROR
            raise ParseIssueError(issue)
        # In LENIENT mode, collect warning instead of silently ignoring
        warnings.append(issue)

    name = get_string(normalized.get("name"))
    if not name:
        raise ParseIssueError(
            ParseIssue(
                row=row_num,
                severity=ParseIssueSeverity.ERROR,
                code="MISSING_REQUIRED_FIELD",
                message="Name is required",
                column="name",
            )
        )

    email = get_string(normalized.get("email"))
    if email:
        try:
            _email_validator.validate_python(email)
        except ValidationError as exc:
            raise ParseIssueError(
                ParseIssue(
                    row=row_num,
                    severity=ParseIssueSeverity.ERROR,
                    code="INVALID_EMAIL",
                    message=f"Invalid email format: {email}",
                    column="email",
                    value=email,
                )
            ) from exc

    role = get_string(normalized.get("role"))
    if not role:
        raise ParseIssueError(
            ParseIssue(
                row=row_num,
                severity=ParseIssueSeverity.ERROR,
                code="MISSING_REQUIRED_FIELD",
                message="Role is required",
                column="role",
            )
        )

    raw_start_date = normalized.get("start_date")
    try:
        start_date = parse_date(raw_start_date)
    except ValueError as exc:
        raise ParseIssueError(
            ParseIssue(
                row=row_num,
                severity=ParseIssueSeverity.ERROR,
                code="INVALID_DATE",
                message=str(exc),
                column="start_date",
                value=get_string(raw_start_date),
            )
        ) from exc

    entry = EmployeeEntry(
        excel_row=row_num,
        name=name,
        email=email,
        role=role,
        department=get_string(normalized.get("department")),
        start_date=start_date,
    )

    return entry, warnings
