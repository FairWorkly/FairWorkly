// FairWorkly.Application/Settings/Commands/UpdateOrganization/UpdateOrganizationCommand.cs

using MediatR;

namespace FairWorkly.Application.Settings.Commands.UpdateOrganization;

/// <summary>
/// Command to update organization profile
/// Updates organization data from Settings page
/// </summary>
public record UpdateOrganizationCommand : IRequest<OrganizationDto>
{
    /// <summary>
    /// Organization ID from JWT claims
    /// Set by the controller after reading JWT
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Updated organization data from client
    /// </summary>
    public UpdateOrganizationRequest Request { get; init; } = null!;
}