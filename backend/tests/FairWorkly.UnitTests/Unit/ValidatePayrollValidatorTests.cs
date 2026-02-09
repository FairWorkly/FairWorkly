using FairWorkly.Application.Payroll.Features.ValidatePayroll;
using FluentAssertions;

namespace FairWorkly.UnitTests.Unit;

public class ValidatePayrollValidatorTests
{
    private readonly ValidatePayrollValidator _validator = new();

    private static ValidatePayrollCommand CreateValidCommand() => new()
    {
        FileStream = new MemoryStream(new byte[] { 1 }),
        FileName = "test.csv",
        FileSize = 1024,
        AwardType = "GeneralRetailIndustryAward2020",
        State = "VIC",
    };

    // ==================== File 验证 ====================

    [Fact]
    public async Task Validate_NoFile_ReturnsError()
    {
        var command = CreateValidCommand();
        command.FileStream = null!;

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "file" &&
            e.ErrorMessage == "File is required");
    }

    [Fact]
    public async Task Validate_NonCsvFile_ReturnsError()
    {
        var command = CreateValidCommand();
        command.FileName = "data.txt";

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "file" &&
            e.ErrorMessage == "File must be a CSV file (.csv)");
    }

    [Fact]
    public async Task Validate_FileOver50MB_ReturnsError()
    {
        var command = CreateValidCommand();
        command.FileSize = 51 * 1024 * 1024L;

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "file" &&
            e.ErrorMessage == "File size must not exceed 50MB");
    }

    // ==================== AwardType 验证 ====================

    [Fact]
    public async Task Validate_MissingAwardType_ReturnsError()
    {
        var command = CreateValidCommand();
        command.AwardType = "";

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "awardType" &&
            e.ErrorMessage == "Award type is required");
    }

    [Fact]
    public async Task Validate_InvalidAwardType_ReturnsError()
    {
        var command = CreateValidCommand();
        command.AwardType = "FastFood";

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "awardType" &&
            e.ErrorMessage.Contains("must be one of"));
    }

    // ==================== State 验证 ====================

    [Fact]
    public async Task Validate_InvalidState_ReturnsError()
    {
        var command = CreateValidCommand();
        command.State = "California";

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "state" &&
            e.ErrorMessage.Contains("must be one of"));
    }

    // ==================== Happy Path ====================

    [Fact]
    public async Task Validate_ValidCommand_PassesValidation()
    {
        var command = CreateValidCommand();

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}
