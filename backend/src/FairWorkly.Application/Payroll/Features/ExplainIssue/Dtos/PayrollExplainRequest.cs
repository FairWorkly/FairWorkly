namespace FairWorkly.Application.Payroll.Features.ExplainIssue.Dtos;

public class PayrollExplainRequest
{
    public required Guid IssueId { get; init; }
    public required string CategoryType { get; init; }
    public required string EmployeeName { get; init; }
    public required string EmployeeId { get; init; }
    public required string Severity { get; init; }
    public required decimal ImpactAmount { get; init; }
    public PayrollExplainDescription? Description { get; init; }
    public string? Warning { get; init; }
}

public class PayrollExplainDescription
{
    public required decimal ActualValue { get; init; }
    public required decimal ExpectedValue { get; init; }
    public required decimal AffectedUnits { get; init; }
    public required string UnitType { get; init; }
    public required string ContextLabel { get; init; }
}
