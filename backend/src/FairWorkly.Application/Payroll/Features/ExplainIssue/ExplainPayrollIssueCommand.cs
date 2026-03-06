using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.Payroll.Features.ExplainIssue;

/// <summary>
/// Frontend POSTs a single issue from the validation result as-is.
/// Field structure matches IssueDto from /api/payroll/validation response.
/// </summary>
public class ExplainPayrollIssueCommand : IRequest<Result<ExplainPayrollIssueDto>>
{
    public Guid IssueId { get; set; }
    public string CategoryType { get; set; } = "";
    public string EmployeeName { get; set; } = "";
    public string EmployeeId { get; set; } = "";
    public int Severity { get; set; }
    public decimal ImpactAmount { get; set; }
    public ExplainIssueDescriptionInput? Description { get; set; }
    public string? Warning { get; set; }
}

/// <summary>
/// Input sub-object matching IssueDescriptionDto structure.
/// Named "Input" (not "Dto") — this is a command input, not a response DTO.
/// </summary>
public class ExplainIssueDescriptionInput
{
    public decimal ActualValue { get; set; }
    public decimal ExpectedValue { get; set; }
    public decimal AffectedUnits { get; set; }
    public string UnitType { get; set; } = "";
    public string ContextLabel { get; set; } = "";
}
