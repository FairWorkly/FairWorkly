namespace FairWorkly.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? OrganizationId { get; }
    string? Email { get; }
}
