using FairWorkly.Application.Settings.Features.UpdateTeamMember;
using FluentAssertions;
using Xunit;

namespace FairWorkly.UnitTests.Settings;

public class UpdateTeamMemberValidatorTests
{
    [Theory]
    [InlineData("Admin", true)]
    [InlineData("Manager", true)]
    [InlineData("SuperAdmin", false)]
    [InlineData("Employee", false)]
    [InlineData("", false)]
    public void Validate_RoleValues(string role, bool shouldBeValid)
    {
        // Arrange
        var validator = new UpdateTeamMemberValidator();
        var command = new UpdateTeamMemberCommand
        {
            UserId = Guid.NewGuid(),
            RequestingUserOrganizationId = Guid.NewGuid(),
            Role = role
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().Be(shouldBeValid);
    }
}
