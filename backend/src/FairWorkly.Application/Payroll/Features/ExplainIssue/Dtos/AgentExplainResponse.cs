namespace FairWorkly.Application.Payroll.Features.ExplainIssue.Dtos;

/// <summary>
/// Python agent-service response envelope: { code, msg, data }
/// </summary>
public class AgentExplainResponse
{
    public required int Code { get; init; }
    public required string Msg { get; init; }
    public AgentExplainData? Data { get; init; }
}

/// <summary>
/// The data payload: AI-generated explanation + recommendation + sources
/// </summary>
public class AgentExplainData
{
    public string? Type { get; init; }
    public required string DetailedExplanation { get; init; }
    public required string Recommendation { get; init; }
    public string? Model { get; init; }
    public List<AgentSourceItem>? Sources { get; init; }
    public string? Note { get; init; }
}

/// <summary>
/// RAG retrieval source reference
/// </summary>
public class AgentSourceItem
{
    public required string Source { get; init; }
    public int? Page { get; init; }
    public string? Content { get; init; }
}
