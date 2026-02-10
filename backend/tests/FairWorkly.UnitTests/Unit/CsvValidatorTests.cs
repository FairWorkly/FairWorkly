using System.Text;
using FairWorkly.Application.Payroll.Services;
using FairWorkly.Application.Payroll.Services.Models;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Payroll;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace FairWorkly.UnitTests.Unit;

public class CsvValidatorTests
{
    private const string ValidHeader =
        "Employee ID,First Name,Last Name,Pay Period Start,Pay Period End,Pay Date," +
        "Award Type,Classification,Employment Type,Hourly Rate," +
        "Ordinary Hours,Ordinary Pay,Saturday Hours,Saturday Pay," +
        "Sunday Hours,Sunday Pay,Public Holiday Hours,Public Holiday Pay," +
        "Gross Pay,Superannuation Paid";

    private readonly CsvValidator _validator;

    public CsvValidatorTests()
    {
        _validator = new CsvValidator();
    }

    // ==================== Helper ====================

    private static List<string[]> LoadCsvFile(string fileName)
    {
        var path = Path.Combine("TestData", "Csv", "CsvValidator", fileName);
        using var stream = File.OpenRead(path);
        var parser = new CsvParser(NullLogger<CsvParser>.Instance);
        var result = parser.Parse(stream);
        return result.Value!;
    }

    private static List<string[]> ParseInlineCsv(string csv)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        var parser = new CsvParser(NullLogger<CsvParser>.Instance);
        var result = parser.Parse(stream);
        return result.Value!;
    }

    private static CsvValidationError AsCsvError(ValidationError error) => (CsvValidationError)error;

    // ==================== Stage 1: Header validation ====================

    [Fact]
    public void Validate_19Columns_ReturnsHeaderError()
    {
        // Arrange — v1: missing Pay Date column (19 columns)
        var rows = LoadCsvFile("v1_wrong_header.csv");

        // Act
        var result = _validator.Validate(rows, AwardType.GeneralRetailIndustryAward2020);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ValidationErrors.Should().HaveCount(1);
        var error = AsCsvError(result.ValidationErrors![0]);
        error.RowNumber.Should().Be(1);
        error.Field.Should().Be("Header");
        error.Message.Should().Be("Expected 20 columns, found 19");
    }

    [Fact]
    public void Validate_21Columns_ReturnsHeaderError()
    {
        // Arrange — 21 columns (extra column added)
        var header = ValidHeader + ",Extra";
        var dataRow = "E001,John,Smith,2026-01-01,2026-01-07,2026-01-10,Retail,Level 2,full-time,27.16,38,1032.08,0,,0,,0,,1032.08,123.85,extra";
        var rows = ParseInlineCsv(header + "\n" + dataRow);

        // Act
        var result = _validator.Validate(rows, AwardType.GeneralRetailIndustryAward2020);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ValidationErrors.Should().HaveCount(1);
        var error = AsCsvError(result.ValidationErrors![0]);
        error.RowNumber.Should().Be(1);
        error.Field.Should().Be("Header");
        error.Message.Should().Be("Expected 20 columns, found 21");
    }

    [Fact]
    public void Validate_WrongColumnName_ReturnsHeaderError()
    {
        // Arrange — 20 columns but column 2 name wrong ("Employee Name" instead of "First Name")
        var wrongHeader = ValidHeader.Replace("First Name", "Employee Name");
        var dataRow = "E001,John,Smith,2026-01-01,2026-01-07,2026-01-10,Retail,Level 2,full-time,27.16,38,1032.08,0,,0,,0,,1032.08,123.85";
        var rows = ParseInlineCsv(wrongHeader + "\n" + dataRow);

        // Act
        var result = _validator.Validate(rows, AwardType.GeneralRetailIndustryAward2020);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ValidationErrors.Should().HaveCount(1);
        var error = AsCsvError(result.ValidationErrors![0]);
        error.RowNumber.Should().Be(1);
        error.Field.Should().Be("Header");
        error.Message.Should().Be("Column 2: expected \"First Name\", found \"Employee Name\"");
    }

    // ==================== Stage 2: Global validation ====================

    [Fact]
    public void Validate_PayPeriodInconsistentAndDuplicateId_ReturnsBothErrors()
    {
        // Arrange — v2: inconsistent Pay Period + duplicate Employee ID
        var rows = LoadCsvFile("v2_global_errors.csv");

        // Act
        var result = _validator.Validate(rows, AwardType.GeneralRetailIndustryAward2020);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ValidationErrors.Should().HaveCount(2);

        var errors = result.ValidationErrors!.Cast<CsvValidationError>().ToList();

        errors.Should().Contain(e =>
            e.RowNumber == 0 &&
            e.Field == "Pay Period" &&
            e.Message == "Pay Period must be the same for all rows");

        errors.Should().Contain(e =>
            e.RowNumber == 0 &&
            e.Field == "Employee ID" &&
            e.Message.Contains("Duplicate Employee ID"));
    }

    [Fact]
    public void Validate_InvalidPayPeriodFormat_ReturnsFormatError()
    {
        // Arrange — consistent Pay Period but invalid format "2026-13-01"
        var csv = ValidHeader + "\n" +
            "E001,John,Smith,2026-13-01,2026-13-07,2026-01-10,Retail,Level 2,full-time,27.16,38,1032.08,0,,0,,0,,1032.08,123.85";
        var rows = ParseInlineCsv(csv);

        // Act
        var result = _validator.Validate(rows, AwardType.GeneralRetailIndustryAward2020);

        // Assert
        result.IsFailure.Should().BeTrue();
        var error = AsCsvError(result.ValidationErrors![0]);
        error.RowNumber.Should().Be(0);
        error.Field.Should().Be("Pay Period");
        error.Message.Should().Contain("Invalid Pay Period format");
    }

    [Fact]
    public void Validate_PayPeriodStartAfterEnd_ReturnsLogicError()
    {
        // Arrange — Pay Period Start > End
        var csv = ValidHeader + "\n" +
            "E001,John,Smith,2026-01-07,2026-01-01,2026-01-10,Retail,Level 2,full-time,27.16,38,1032.08,0,,0,,0,,1032.08,123.85";
        var rows = ParseInlineCsv(csv);

        // Act
        var result = _validator.Validate(rows, AwardType.GeneralRetailIndustryAward2020);

        // Assert
        result.IsFailure.Should().BeTrue();
        var error = AsCsvError(result.ValidationErrors![0]);
        error.RowNumber.Should().Be(0);
        error.Field.Should().Be("Pay Period");
        error.Message.Should().Be("Invalid Pay Period. Start date must be on or before end date");
    }

    [Fact]
    public void Validate_NoDataRows_ReturnsError()
    {
        // Arrange — header row only
        var rows = ParseInlineCsv(ValidHeader);

        // Act
        var result = _validator.Validate(rows, AwardType.GeneralRetailIndustryAward2020);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ValidationErrors.Should().HaveCount(1);
        var error = AsCsvError(result.ValidationErrors![0]);
        error.RowNumber.Should().Be(0);
        error.Field.Should().Be("File");
        error.Message.Should().Be("CSV file has no data rows");
    }

    // ==================== Stage 3: Field-level validation ====================

    [Fact]
    public void Validate_FieldErrors_Returns19Errors()
    {
        // Arrange — v3: 10 data rows, 9 with errors (2 errors each), 1 correct
        var rows = LoadCsvFile("v3_field_errors.csv");

        // Act
        var result = _validator.Validate(rows, AwardType.GeneralRetailIndustryAward2020);

        // Assert
        result.IsFailure.Should().BeTrue();
        var errors = result.ValidationErrors!.Cast<CsvValidationError>().ToList();

        // Row 2: Employee ID empty + Last Name empty
        errors.Should().Contain(e => e.RowNumber == 2 && e.Message == "Employee ID is required");
        errors.Should().Contain(e => e.RowNumber == 2 && e.Message == "Last Name is required");

        // Row 3: First Name empty + Pay Date format error
        errors.Should().Contain(e => e.RowNumber == 3 && e.Message == "First Name is required");
        errors.Should().Contain(e => e.RowNumber == 3 && e.Message.Contains("Invalid Pay Date format"));

        // Row 4: Award Type mismatch + Classification invalid
        errors.Should().Contain(e => e.RowNumber == 4 && e.Message.Contains("Award Type is required"));
        errors.Should().Contain(e => e.RowNumber == 4 && e.Message.Contains("Classification is required"));

        // Row 5: Employment Type invalid + Hourly Rate parse failure
        errors.Should().Contain(e => e.RowNumber == 5 && e.Message.Contains("Employment Type is required"));
        errors.Should().Contain(e => e.RowNumber == 5 && e.Message == "Hourly Rate must be a positive number");

        // Row 6: Hourly Rate = 0 + Ordinary Hours = -5
        errors.Should().Contain(e => e.RowNumber == 6 && e.Message == "Hourly Rate must be a positive number");
        errors.Should().Contain(e => e.RowNumber == 6 && e.Message == "Ordinary Hours must be a number >= 0");

        // Row 7: Saturday Pay conditionally required + Sunday Pay conditionally required
        errors.Should().Contain(e => e.RowNumber == 7 && e.Message == "Saturday Pay is required when Saturday Hours > 0");
        errors.Should().Contain(e => e.RowNumber == 7 && e.Message == "Sunday Pay is required when Sunday Hours > 0");

        // Row 8: Ordinary Pay parse failure + Public Holiday Pay conditionally required
        errors.Should().Contain(e => e.RowNumber == 8 && e.Message == "Ordinary Pay must be a number");
        errors.Should().Contain(e => e.RowNumber == 8 && e.Message == "Public Holiday Pay is required when Public Holiday Hours > 0");

        // Row 9: Gross Pay parse failure + Super parse failure
        errors.Should().Contain(e => e.RowNumber == 9 && e.Message == "Gross Pay must be a number");
        errors.Should().Contain(e => e.RowNumber == 9 && e.Message == "Superannuation Paid must be a number");

        // Row 10: Saturday Hours negative + Sunday Hours non-numeric
        errors.Should().Contain(e => e.RowNumber == 10 && e.Message == "Saturday Hours must be a number >= 0");
        errors.Should().Contain(e => e.RowNumber == 10 && e.Message == "Sunday Hours must be a number >= 0");

        // 18 errors total (9 rows × 2 errors/row)
        errors.Should().HaveCount(18);
    }

    // ==================== Happy Path ====================

    [Fact]
    public void Validate_AllValid_ReturnsValidatedRows()
    {
        // Arrange — v4: 6 rows of valid data
        var rows = LoadCsvFile("v4_happy_path.csv");

        // Act
        var result = _validator.Validate(rows, AwardType.GeneralRetailIndustryAward2020);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(6);

        // Verify row 1: full-time, no optional fields
        var row1 = result.Value![0];
        row1.EmployeeId.Should().Be("H001");
        row1.FirstName.Should().Be("Alice");
        row1.LastName.Should().Be("Smith");
        row1.PayPeriodStart.Should().Be(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
        row1.PayPeriodEnd.Should().Be(new DateTimeOffset(2026, 1, 7, 0, 0, 0, TimeSpan.Zero));
        row1.PayDate.Should().Be(new DateTimeOffset(2026, 1, 10, 0, 0, 0, TimeSpan.Zero));
        row1.AwardType.Should().Be("Retail");
        row1.Classification.Should().Be("Level 1");
        row1.EmploymentType.Should().Be(EmploymentType.FullTime);
        row1.HourlyRate.Should().Be(26.55m);
        row1.OrdinaryHours.Should().Be(38m);
        row1.OrdinaryPay.Should().Be(1008.90m);
        row1.SaturdayHours.Should().Be(0m);
        row1.SaturdayPay.Should().Be(0m);
        row1.GrossPay.Should().Be(1008.90m);
        row1.Superannuation.Should().Be(121.07m);

        // Verify row 3: casual, with Saturday + Sunday
        var row3 = result.Value[2];
        row3.EmploymentType.Should().Be(EmploymentType.Casual);
        row3.SaturdayHours.Should().Be(8m);
        row3.SaturdayPay.Should().Be(344.80m);
        row3.SundayHours.Should().Be(4m);
        row3.SundayPay.Should().Be(172.40m);

        // Verify row 5: full-time, all optional fields present
        var row5 = result.Value[4];
        row5.SaturdayHours.Should().Be(4m);
        row5.SundayHours.Should().Be(4m);
        row5.PublicHolidayHours.Should().Be(8m);
        row5.PublicHolidayPay.Should().Be(292.70m);
    }
}
