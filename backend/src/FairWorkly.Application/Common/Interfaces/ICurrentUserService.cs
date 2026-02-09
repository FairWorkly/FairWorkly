namespace FairWorkly.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? OrganizationId { get; }
    string? Email { get; }
    string? Role { get; }
    Guid? EmployeeId { get; }
}
