using FairWorkly.Application.Settings.Features.GetTeamMembers;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Auth.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace FairWorkly.UnitTests.Settings;

public class GetTeamMembersHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsOnlyUsersFromSameOrganization()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var users = new List<User>
        {
            new User { Id = Guid.NewGuid(), FirstName = "User1", LastName = "Test", OrganizationId = orgId, IsActive = true },
            new User { Id = Guid.NewGuid(), FirstName = "User2", LastName = "Test", OrganizationId = orgId, IsActive = false }
        };

        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetByOrganizationIdAsync(orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var handler = new GetTeamMembersHandler(mockRepo.Object);
        var query = new GetTeamMembersQuery { OrganizationId = orgId };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        mockRepo.Verify(r => r.GetByOrganizationIdAsync(orgId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_FiltersOutDeletedUsers()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var users = new List<User>
        {
            new User { Id = Guid.NewGuid(), FirstName = "Valid", LastName = "User", OrganizationId = orgId, IsDeleted = false },
            new User { Id = Guid.NewGuid(), FirstName = "Deleted", LastName = "User", OrganizationId = orgId, IsDeleted = true }
        };

        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetByOrganizationIdAsync(orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var handler = new GetTeamMembersHandler(mockRepo.Object);
        var query = new GetTeamMembersQuery { OrganizationId = orgId };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Valid User");
    }
}
