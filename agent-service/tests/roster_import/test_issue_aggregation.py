from agents.roster.services.roster_import.issues import build_summary, build_issue_summary, is_blocking_issue
from agents.roster.services.roster_import import ParseIssue, ParseIssueSeverity, ParseResultStatus

def test_build_summary_statuses():
    issues = [
        ParseIssue(row=0, severity=ParseIssueSeverity.ERROR, code="EMPTY_FILE", message="empty"),
    ]
    summary = build_summary(issues)
    assert summary.status == ParseResultStatus.BLOCKING

    issues = [
        ParseIssue(row=2, severity=ParseIssueSeverity.ERROR, code="INVALID_DATE", message="bad date"),
    ]
    summary = build_summary(issues)
    assert summary.status == ParseResultStatus.ROW_ERROR

    issues = [
        ParseIssue(row=2, severity=ParseIssueSeverity.WARNING, code="WARN", message="warn"),
    ]
    summary = build_summary(issues)
    assert summary.status == ParseResultStatus.WARNING

    summary = build_summary([])
    assert summary.status == ParseResultStatus.OK

def test_issue_summary_grouping_sorting():
    issues = [
        ParseIssue(row=2, severity=ParseIssueSeverity.ERROR, code="A_ERROR", message="a", column="date"),
        ParseIssue(row=3, severity=ParseIssueSeverity.ERROR, code="A_ERROR", message="a2", column="date"),
        ParseIssue(row=4, severity=ParseIssueSeverity.ERROR, code="Z_ERROR", message="z", column="date"),
        ParseIssue(row=5, severity=ParseIssueSeverity.WARNING, code="B_WARN", message="b", column="date"),
        ParseIssue(row=6, severity=ParseIssueSeverity.WARNING, code="B_WARN", message="b2", column="date"),
        ParseIssue(row=7, severity=ParseIssueSeverity.WARNING, code="C_WARN", message="c", column="date"),
    ]

    summary = build_issue_summary(issues)
    assert [group.code for group in summary] == ["A_ERROR", "Z_ERROR", "B_WARN", "C_WARN"]
    assert summary[0].count == 2
    assert summary[0].sample_rows == [2, 3]

def test_blocking_rule():
    blocking = ParseIssue(row=0, severity=ParseIssueSeverity.ERROR, code="EMPTY_FILE", message="empty")
    non_blocking = ParseIssue(row=2, severity=ParseIssueSeverity.ERROR, code="INVALID", message="bad")

    assert is_blocking_issue(blocking) is True
    assert is_blocking_issue(non_blocking) is False
