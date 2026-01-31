using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Settings.Features.UpdateTeamMember;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Auth.Enums;
using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace FairWorkly.UnitTests.Settings;

public class UpdateTeamMemberHandlerTests
{
    private readonly Mock<IUserRepository> _mockRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly UpdateTeamMemberHandler _handler;

    public UpdateTeamMemberHandlerTests()
    {
        _mockRepo = new Mock<IUserRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new UpdateTeamMemberHandler(_mockRepo.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_UpdatesUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var orgId = Guid.NewGuid();
        var user = new User { Id = userId, OrganizationId = orgId, Role = UserRole.Admin };

        _mockRepo.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new UpdateTeamMemberCommand
        {
            UserId = userId,
            RequestingUserOrganizationId = orgId,
            Role = "Manager",
            IsActive = false
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Role.Should().Be("Manager");
        result.IsActive.Should().BeFalse();
        user.Role.Should().Be(UserRole.Manager);
        user.IsActive.Should().BeFalse();
        _mockRepo.Verify(r => r.Update(user), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DifferentOrg_ThrowsForbidden()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, OrganizationId = Guid.NewGuid() }; // Different Org

        _mockRepo.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new UpdateTeamMemberCommand
        {
            UserId = userId,
            RequestingUserOrganizationId = Guid.NewGuid() // Requesting Org
        };

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenAccessException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsNotFound()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new UpdateTeamMemberCommand { UserId = Guid.NewGuid() };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }
}
