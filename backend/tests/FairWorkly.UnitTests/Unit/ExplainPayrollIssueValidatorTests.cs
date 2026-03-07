using FairWorkly.Application.Payroll.Features.ExplainIssue;
using FluentAssertions;

namespace FairWorkly.UnitTests.Unit;

public class ExplainPayrollIssueValidatorTests
{
    private readonly ExplainPayrollIssueValidator _validator = new();

    private static ExplainPayrollIssueCommand CreateValidCommand() =>
        new()
        {
            IssueId = Guid.NewGuid(),
            CategoryType = "PenaltyRate",
            EmployeeName = "Alice Smith",
            EmployeeId = "E001",
            Severity = 3,
            ImpactAmount = 31.6m,
            Description = new ExplainIssueDescriptionInput
            {
                ActualValue = 30m,
                ExpectedValue = 33.95m,
                AffectedUnits = 8m,
                UnitType = "Hour",
                ContextLabel = "Saturday (125% rate)",
            },
        };

    // ==================== Validation failures ====================

    [Fact]
    public async Task Validate_EmptyIssueId_ReturnsError()
    {
        var command = CreateValidCommand();
        command.IssueId = Guid.Empty;

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "IssueId");
    }

    [Fact]
    public async Task Validate_EmptyCategoryType_ReturnsError()
    {
        var command = CreateValidCommand();
        command.CategoryType = "";

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CategoryType");
    }

    [Fact]
    public async Task Validate_InvalidCategoryType_ReturnsError()
    {
        var command = CreateValidCommand();
        command.CategoryType = "Overtime";

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result
            .Errors.Should()
            .Contain(e =>
                e.PropertyName == "CategoryType" && e.ErrorMessage.Contains("must be one of")
            );
    }

    [Fact]
    public async Task Validate_EmptyEmployeeName_ReturnsError()
    {
        var command = CreateValidCommand();
        command.EmployeeName = "";

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "EmployeeName");
    }

    [Fact]
    public async Task Validate_EmptyEmployeeId_ReturnsError()
    {
        var command = CreateValidCommand();
        command.EmployeeId = "";

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "EmployeeId");
    }

    // ==================== Validation success ====================

    [Fact]
    public async Task Validate_AllFieldsValid_Passes()
    {
        var command = CreateValidCommand();

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_DescriptionNull_Passes()
    {
        var command = CreateValidCommand();
        command.Description = null;

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }
}
