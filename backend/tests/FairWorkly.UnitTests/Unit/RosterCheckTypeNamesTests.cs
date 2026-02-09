using FairWorkly.Domain.Roster.Enums;
using FluentAssertions;
using Xunit;

namespace FairWorkly.UnitTests.Unit;

public class RosterCheckTypeNamesTests
{
    [Fact]
    public void RosterCheckType_Names_AreBackwardCompatible_WhenPersistedAsStrings()
    {
        // When using EF Core HasConversion<string>(), enum names are stored in the DB.
        // Renaming/deleting members breaks parsing and historical data.
        var expectedExistingNames = new[]
        {
            "DataQuality",
            "MinimumShiftHours",
            "MealBreak",
            "RestPeriodBetweenShifts",
            "WeeklyHoursLimit",
            "MaximumConsecutiveDays",
        };

        var actualNames = Enum.GetNames(typeof(RosterCheckType));
        actualNames.Should().Contain(expectedExistingNames);
    }
}
