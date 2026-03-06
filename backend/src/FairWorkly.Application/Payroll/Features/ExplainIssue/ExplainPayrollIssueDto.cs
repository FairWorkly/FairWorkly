namespace FairWorkly.Application.Payroll.Features.ExplainIssue;

/// <summary>
/// AI explanation result returned to the frontend.
/// Aligned with API contract Part A data structure (ExplainResult).
/// </summary>
public class ExplainPayrollIssueDto
{
    public Guid IssueId { get; set; }
    public string? DetailedExplanation { get; set; }
    public string? Recommendation { get; set; }
    public string? Model { get; set; }
    public List<ExplainSourceDto> Sources { get; set; } = [];
    public string? Warning { get; set; }
}

/// <summary>
/// RAG retrieval source (pass-through from agent-service, not stored in DB).
/// </summary>
public class ExplainSourceDto
{
    public string Source { get; set; } = "";
    public int? Page { get; set; }
    public string? Content { get; set; }
}
