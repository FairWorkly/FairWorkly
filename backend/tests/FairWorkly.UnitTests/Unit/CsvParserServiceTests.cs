using FairWorkly.Application.Payroll.Services;
using FluentAssertions;
using Xunit;

namespace FairWorkly.UnitTests.Unit;

/// <summary>
/// Unit tests for CsvParserService
/// Tests CSV parsing functionality with various scenarios
/// </summary>
public class CsvParserServiceTests
{
    private readonly CsvParserService _csvParserService;

    public CsvParserServiceTests()
    {
        _csvParserService = new CsvParserService();
    }

    [Fact]
    public async Task ParseAsync_ValidCsv_ReturnsRows()
    {
        // Arrange
        var csvContent = @"Employee ID,Employee Name,Pay Period Start,Pay Period End,Award Type,Classification,Employment Type,Hourly Rate,Ordinary Hours,Ordinary Pay,Saturday Hours,Saturday Pay,Sunday Hours,Sunday Pay,Public Holiday Hours,Public Holiday Pay,Gross Pay,Superannuation Paid
NEW001,Alice Johnson,2025-12-15,2025-12-21,Retail,Level 1,FullTime,26.55,38.00,1008.90,0.00,0.00,0.00,0.00,0.00,0.00,1008.90,121.07
NEW002,Bob Williams,2025-12-15,2025-12-21,Retail,Level 2,PartTime,27.16,20.00,543.20,0.00,0.00,0.00,0.00,0.00,0.00,543.20,65.18";

        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        await writer.WriteAsync(csvContent);
        await writer.FlushAsync();
        stream.Position = 0;

        // Act
        var (rows, errors) = await _csvParserService.ParseAsync(stream);

        // Assert
        errors.Should().BeEmpty();
        rows.Should().HaveCount(2);

        var firstRow = rows[0];
        firstRow.EmployeeId.Should().Be("NEW001");
        firstRow.EmployeeName.Should().Be("Alice Johnson");
        firstRow.PayPeriodStart.Should().Be(new DateOnly(2025, 12, 15));
        firstRow.PayPeriodEnd.Should().Be(new DateOnly(2025, 12, 21));
        firstRow.AwardType.Should().Be("Retail");
        firstRow.Classification.Should().Be("Level 1");
        firstRow.EmploymentType.Should().Be("FullTime");
        firstRow.HourlyRate.Should().Be(26.55m);
        firstRow.OrdinaryHours.Should().Be(38.00m);
        firstRow.OrdinaryPay.Should().Be(1008.90m);
        firstRow.SaturdayHours.Should().Be(0.00m);
        firstRow.SaturdayPay.Should().Be(0.00m);
        firstRow.SundayHours.Should().Be(0.00m);
        firstRow.SundayPay.Should().Be(0.00m);
        firstRow.PublicHolidayHours.Should().Be(0.00m);
        firstRow.PublicHolidayPay.Should().Be(0.00m);
        firstRow.GrossPay.Should().Be(1008.90m);
        firstRow.SuperannuationPaid.Should().Be(121.07m);
    }

    [Fact]
    public async Task ParseAsync_MissingRequiredField_ReturnsError()
    {
        // Arrange - Missing Employee ID in second row
        var csvContent = @"Employee ID,Employee Name,Pay Period Start,Pay Period End,Award Type,Classification,Employment Type,Hourly Rate,Ordinary Hours,Ordinary Pay,Saturday Hours,Saturday Pay,Sunday Hours,Sunday Pay,Public Holiday Hours,Public Holiday Pay,Gross Pay,Superannuation Paid
NEW001,Alice Johnson,2025-12-15,2025-12-21,Retail,Level 1,FullTime,26.55,38.00,1008.90,0.00,0.00,0.00,0.00,0.00,0.00,1008.90,121.07
,Bob Williams,2025-12-15,2025-12-21,Retail,Level 2,PartTime,27.16,20.00,543.20,0.00,0.00,0.00,0.00,0.00,0.00,543.20,65.18";

        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        await writer.WriteAsync(csvContent);
        await writer.FlushAsync();
        stream.Position = 0;

        // Act
        var (rows, errors) = await _csvParserService.ParseAsync(stream);

        // Assert
        errors.Should().NotBeEmpty();
        errors.Should().Contain(e => e.Contains("Row 3") && e.Contains("Employee ID is required"));
        rows.Should().HaveCount(1); // Only valid row should be returned
    }

    [Fact]
    public async Task ParseAsync_OptionalFieldsMissing_UsesDefaultValues()
    {
        // Arrange - CSV without optional weekend/holiday fields
        var csvContent = @"Employee ID,Employee Name,Pay Period Start,Pay Period End,Award Type,Classification,Employment Type,Hourly Rate,Ordinary Hours,Ordinary Pay,Gross Pay,Superannuation Paid
NEW001,Alice Johnson,2025-12-15,2025-12-21,Retail,Level 1,FullTime,26.55,38.00,1008.90,1008.90,121.07";

        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        await writer.WriteAsync(csvContent);
        await writer.FlushAsync();
        stream.Position = 0;

        // Act
        var (rows, errors) = await _csvParserService.ParseAsync(stream);

        // Assert
        errors.Should().BeEmpty();
        rows.Should().HaveCount(1);

        var row = rows[0];
        row.SaturdayHours.Should().Be(0m);
        row.SaturdayPay.Should().Be(0m);
        row.SundayHours.Should().Be(0m);
        row.SundayPay.Should().Be(0m);
        row.PublicHolidayHours.Should().Be(0m);
        row.PublicHolidayPay.Should().Be(0m);
    }

    [Fact]
    public async Task ParseAsync_EmptyStream_ReturnsEmptyList()
    {
        // Arrange
        using var stream = new MemoryStream();

        // Act
        var (rows, errors) = await _csvParserService.ParseAsync(stream);

        // Assert
        rows.Should().BeEmpty();
        errors.Should().NotBeEmpty(); // Should have error about parsing failure
    }

    [Fact]
    public async Task ParseAsync_InvalidDateFormat_ReturnsError()
    {
        // Arrange - Invalid date format
        var csvContent = @"Employee ID,Employee Name,Pay Period Start,Pay Period End,Award Type,Classification,Employment Type,Hourly Rate,Ordinary Hours,Ordinary Pay,Saturday Hours,Saturday Pay,Sunday Hours,Sunday Pay,Public Holiday Hours,Public Holiday Pay,Gross Pay,Superannuation Paid
NEW001,Alice Johnson,15/12/2025,21/12/2025,Retail,Level 1,FullTime,26.55,38.00,1008.90,0.00,0.00,0.00,0.00,0.00,0.00,1008.90,121.07";

        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        await writer.WriteAsync(csvContent);
        await writer.FlushAsync();
        stream.Position = 0;

        // Act
        var (rows, errors) = await _csvParserService.ParseAsync(stream);

        // Assert
        errors.Should().NotBeEmpty();
        rows.Should().BeEmpty();
    }

    [Fact]
    public async Task ParseAsync_NegativeHourlyRate_ReturnsError()
    {
        // Arrange - Negative hourly rate
        var csvContent = @"Employee ID,Employee Name,Pay Period Start,Pay Period End,Award Type,Classification,Employment Type,Hourly Rate,Ordinary Hours,Ordinary Pay,Saturday Hours,Saturday Pay,Sunday Hours,Sunday Pay,Public Holiday Hours,Public Holiday Pay,Gross Pay,Superannuation Paid
NEW001,Alice Johnson,2025-12-15,2025-12-21,Retail,Level 1,FullTime,-26.55,38.00,1008.90,0.00,0.00,0.00,0.00,0.00,0.00,1008.90,121.07";

        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        await writer.WriteAsync(csvContent);
        await writer.FlushAsync();
        stream.Position = 0;

        // Act
        var (rows, errors) = await _csvParserService.ParseAsync(stream);

        // Assert
        errors.Should().NotBeEmpty();
        errors.Should().Contain(e => e.Contains("Hourly Rate must be greater than 0"));
        rows.Should().BeEmpty();
    }

    [Fact]
    public async Task ParseAsync_MissingEmployeeId_ReturnsError()
    {
        // Arrange - First row has empty Employee ID
        var csvContent = @"Employee ID,Employee Name,Pay Period Start,Pay Period End,Award Type,Classification,Employment Type,Hourly Rate,Ordinary Hours,Ordinary Pay,Saturday Hours,Saturday Pay,Sunday Hours,Sunday Pay,Public Holiday Hours,Public Holiday Pay,Gross Pay,Superannuation Paid
,Alice Johnson,2025-12-15,2025-12-21,Retail,Level 1,FullTime,26.55,38.00,1008.90,0.00,0.00,0.00,0.00,0.00,0.00,1008.90,121.07";

        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        await writer.WriteAsync(csvContent);
        await writer.FlushAsync();
        stream.Position = 0;

        // Act
        var (rows, errors) = await _csvParserService.ParseAsync(stream);

        // Assert
        errors.Should().NotBeEmpty();
        errors.Should().Contain(e => e.Contains("Employee ID is required"));
    }

    [Fact]
    public async Task ParseAsync_InvalidEmploymentType_ReturnsError()
    {
        // Arrange - Invalid employment type "Contract"
        var csvContent = @"Employee ID,Employee Name,Pay Period Start,Pay Period End,Award Type,Classification,Employment Type,Hourly Rate,Ordinary Hours,Ordinary Pay,Saturday Hours,Saturday Pay,Sunday Hours,Sunday Pay,Public Holiday Hours,Public Holiday Pay,Gross Pay,Superannuation Paid
NEW001,Alice Johnson,2025-12-15,2025-12-21,Retail,Level 1,Contract,26.55,38.00,1008.90,0.00,0.00,0.00,0.00,0.00,0.00,1008.90,121.07";

        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        await writer.WriteAsync(csvContent);
        await writer.FlushAsync();
        stream.Position = 0;

        // Act
        var (rows, errors) = await _csvParserService.ParseAsync(stream);

        // Assert
        errors.Should().NotBeEmpty();
        errors.Should().Contain(e => e.Contains("Employment Type") || e.Contains("Invalid"));
    }

    [Fact]
    public async Task ParseAsync_EmptyFile_ReturnsError()
    {
        // Arrange - Completely empty file (0 bytes)
        using var stream = new MemoryStream(Array.Empty<byte>());

        // Act
        var (rows, errors) = await _csvParserService.ParseAsync(stream);

        // Assert
        rows.Should().BeEmpty();
        errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ParseAsync_HeaderOnly_ReturnsEmptyRows()
    {
        // Arrange - Only header row, no data rows
        var csvContent = @"Employee ID,Employee Name,Pay Period Start,Pay Period End,Award Type,Classification,Employment Type,Hourly Rate,Ordinary Hours,Ordinary Pay,Saturday Hours,Saturday Pay,Sunday Hours,Sunday Pay,Public Holiday Hours,Public Holiday Pay,Gross Pay,Superannuation Paid";

        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        await writer.WriteAsync(csvContent);
        await writer.FlushAsync();
        stream.Position = 0;

        // Act
        var (rows, errors) = await _csvParserService.ParseAsync(stream);

        // Assert
        rows.Should().BeEmpty();
        errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ParseAsync_FromTestFile_TEST_01_NewEmployees()
    {
        // Arrange
        var testFilePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "TestData", "Csv", "EmployeeSync", "TEST_01_NewEmployees.csv");

        if (!File.Exists(testFilePath))
        {
            // Skip test if file doesn't exist in test environment
            return;
        }

        using var stream = File.OpenRead(testFilePath);

        // Act
        var (rows, errors) = await _csvParserService.ParseAsync(stream);

        // Assert
        errors.Should().BeEmpty();
        rows.Should().HaveCount(5);

        // Verify sample data
        rows.Should().Contain(r => r.EmployeeId == "NEW001");
        rows.Should().Contain(r => r.EmployeeId == "NEW002");
        rows.Should().Contain(r => r.EmployeeId == "NEW003");
        rows.Should().Contain(r => r.EmployeeId == "NEW004");
        rows.Should().Contain(r => r.EmployeeId == "NEW005");
    }
}
