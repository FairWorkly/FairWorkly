// FairWorkly.Application/Settings/Queries/GetOrganization/GetOrganizationQuery.cs

using MediatR;

namespace FairWorkly.Application.Settings.Queries.GetOrganization;

/// <summary>
/// Query to get organization profile for the current tenant
/// Returns organization data for Settings page display
/// </summary>
public record GetOrganizationQuery : IRequest<OrganizationDto>
{
    /// <summary>
    /// Organization ID from JWT claims
    /// Set by the controller after reading JWT
    /// </summary>
    public Guid OrganizationId { get; init; }
}