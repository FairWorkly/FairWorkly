namespace FairWorkly.Application.Payroll.Features.ValidatePayroll;

public class ValidatePayrollDto
{
    public Guid ValidationId { get; set; }
    public string Status { get; set; } = "";
    public DateTimeOffset Timestamp { get; set; }
    public string PayPeriodStart { get; set; } = "";
    public string PayPeriodEnd { get; set; } = "";
    public SummaryDto Summary { get; set; } = new();
    public List<CategoryDto> Categories { get; set; } = new();
    public List<IssueDto> Issues { get; set; } = new();
}

public class SummaryDto
{
    public int PassedCount { get; set; }
    public int TotalIssues { get; set; }
    public decimal TotalUnderpayment { get; set; }
    public int AffectedEmployees { get; set; }
}

public class CategoryDto
{
    public string Key { get; set; } = "";
    public int AffectedEmployeeCount { get; set; }
    public decimal TotalUnderpayment { get; set; }
}

public class IssueDto
{
    public Guid IssueId { get; set; }
    public string CategoryType { get; set; } = "";
    public string EmployeeName { get; set; } = "";
    public string EmployeeId { get; set; } = "";
    public int Severity { get; set; }
    public decimal ImpactAmount { get; set; }
    public IssueDescriptionDto? Description { get; set; }
    public string? Warning { get; set; }
}

public class IssueDescriptionDto
{
    public decimal ActualValue { get; set; }
    public decimal ExpectedValue { get; set; }
    public decimal AffectedUnits { get; set; }
    public string UnitType { get; set; } = "";
    public string ContextLabel { get; set; } = "";
}
