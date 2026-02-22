using System.Text;
using FairWorkly.Application.Payroll.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace FairWorkly.UnitTests.Unit;

public class CsvParserTests
{
    private const string ValidHeader =
        "Employee ID,First Name,Last Name,Pay Period Start,Pay Period End,Pay Date,"
        + "Award Type,Classification,Employment Type,Hourly Rate,"
        + "Ordinary Hours,Ordinary Pay,Saturday Hours,Saturday Pay,"
        + "Sunday Hours,Sunday Pay,Public Holiday Hours,Public Holiday Pay,"
        + "Gross Pay,Superannuation Paid";

    private readonly CsvParser _parser;

    public CsvParserTests()
    {
        _parser = new CsvParser(NullLogger<CsvParser>.Instance);
    }

    [Fact]
    public void Parse_EmptyFile_ReturnsFailure()
    {
        // Arrange
        var stream = new MemoryStream();

        // Act
        var result = _parser.Parse(stream, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("CSV file is corrupted or cannot be parsed");
    }

    [Fact]
    public void Parse_CorruptedCsv_ReturnsFailure()
    {
        // Arrange
        var corruptedCsv = "\"unclosed quote,data\n\"another,row";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(corruptedCsv));

        // Act
        var result = _parser.Parse(stream, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("CSV file is corrupted or cannot be parsed");
    }

    [Fact]
    public void Parse_ValidCsv_ReturnsAllRowsIncludingHeader()
    {
        // Arrange
        var csv =
            ValidHeader
            + "\n"
            + "E001,John,Smith,2026-01-01,2026-01-07,2026-01-10,Retail,Level 2,full-time,27.16,38,1032.08,0,,0,,0,,1032.08,123.85\n"
            + "E002,Jane,Doe,2026-01-01,2026-01-07,2026-01-10,Retail,Level 3,casual,34.48,20,689.60,4,172.40,0,,0,,862.00,103.44";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        // Act
        var result = _parser.Parse(stream, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3); // 1 header + 2 data rows
        result.Value[0].Should().HaveCount(20);
        result.Value[0][0].Should().Be("Employee ID");
        result.Value[1][0].Should().Be("E001");
        result.Value[2][0].Should().Be("E002");
    }

    [Fact]
    public void Parse_HeaderOnly_ReturnsListWithHeaderRow()
    {
        // Arrange
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(ValidHeader));

        // Act
        var result = _parser.Parse(stream, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].Should().HaveCount(20);
        result.Value[0][0].Should().Be("Employee ID");
        result.Value[0][19].Should().Be("Superannuation Paid");
    }

    [Fact]
    public void Parse_InconsistentColumnCount_ReturnsFailure()
    {
        // Arrange — header has 20 columns, data row has only 2
        var csv = ValidHeader + "\nE001,John";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        // Act
        var result = _parser.Parse(stream, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(422);
        result.Message.Should().Be("CSV file is corrupted or cannot be parsed");
    }
}
