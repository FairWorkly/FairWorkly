"""Issue aggregation and response builders."""

from __future__ import annotations

from typing import Any, Optional

from .models import (
    ParseIssue,
    ParseIssueSeverity,
    ParseResultStatus,
    ParseResultSummary,
    IssueGroupSummary,
    ParseResponse,
)


def _is_severity(issue: ParseIssue, severity: ParseIssueSeverity) -> bool:
    return issue.severity == severity or issue.severity == severity.value


def is_blocking_issue(issue: ParseIssue) -> bool:
    """Return True if the issue should block the entire import."""
    return issue.row == 0 and _is_severity(issue, ParseIssueSeverity.ERROR)


def build_summary(issues: list[ParseIssue]) -> ParseResultSummary:
    error_count = sum(1 for issue in issues if _is_severity(issue, ParseIssueSeverity.ERROR))
    warning_count = sum(1 for issue in issues if _is_severity(issue, ParseIssueSeverity.WARNING))
    blocking_count = sum(1 for issue in issues if is_blocking_issue(issue))

    if blocking_count > 0:
        status = ParseResultStatus.BLOCKING
    elif error_count > 0:
        status = ParseResultStatus.ROW_ERROR
    elif warning_count > 0:
        status = ParseResultStatus.WARNING
    else:
        status = ParseResultStatus.OK

    return ParseResultSummary(
        status=status,
        total_issues=len(issues),
        error_count=error_count,
        warning_count=warning_count,
        blocking_count=blocking_count,
    )


def build_issue_summary(
    issues: list[ParseIssue],
    sample_size: int = 3,
) -> list[IssueGroupSummary]:
    groups: dict[tuple[str, Optional[str], ParseIssueSeverity], IssueGroupSummary] = {}
    for issue in issues:
        key = (issue.code, issue.column, issue.severity)
        group = groups.get(key)
        if group is None:
            group = IssueGroupSummary(
                code=issue.code,
                column=issue.column,
                severity=issue.severity,
                count=0,
                sample_rows=[],
                sample_messages=[],
            )
            groups[key] = group
        group.count += 1
        if len(group.sample_rows) < sample_size:
            group.sample_rows.append(issue.row)
        if len(group.sample_messages) < sample_size:
            group.sample_messages.append(issue.message)

    def sort_key(group: IssueGroupSummary) -> tuple[int, int, str]:
        severity_rank = 0 if group.severity in (ParseIssueSeverity.ERROR, ParseIssueSeverity.ERROR.value) else 1
        return (severity_rank, -group.count, group.code)

    return sorted(groups.values(), key=sort_key)


def build_response(result: Any, issues: list[ParseIssue]) -> ParseResponse:
    summary = build_summary(issues)
    issue_summary = build_issue_summary(issues)
    return ParseResponse(result=result, issues=issues, summary=summary, issue_summary=issue_summary)
