using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Common.Parsing;
using FluentAssertions;

namespace FairWorkly.UnitTests.Unit;

public class EmploymentTypeParserTests
{
    [Theory]
    [InlineData("full-time", EmploymentType.FullTime)]
    [InlineData("part-time", EmploymentType.PartTime)]
    [InlineData("casual", EmploymentType.Casual)]
    [InlineData("fixed-term", EmploymentType.FixedTerm)]
    [InlineData("Full Time", EmploymentType.FullTime)]
    [InlineData("FULLTIME", EmploymentType.FullTime)]
    [InlineData("full_time", EmploymentType.FullTime)]
    [InlineData("ft", EmploymentType.FullTime)]
    [InlineData("pt", EmploymentType.PartTime)]
    [InlineData("cas", EmploymentType.Casual)]
    public void TryParse_ValidInput_ReturnsTrueWithCorrectType(string input, EmploymentType expected)
    {
        // Act
        var success = EmploymentTypeParser.TryParse(input, out var result);

        // Assert
        success.Should().BeTrue();
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("intern")]
    [InlineData("contractor")]
    [InlineData("")]
    public void TryParse_InvalidInput_ReturnsFalse(string input)
    {
        // Act
        var success = EmploymentTypeParser.TryParse(input, out _);

        // Assert
        success.Should().BeFalse();
    }
}
