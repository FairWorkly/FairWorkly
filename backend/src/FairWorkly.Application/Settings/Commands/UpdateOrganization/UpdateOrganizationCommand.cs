using FairWorkly.Domain.Common;
using MediatR;

namespace FairWorkly.Application.Settings.Commands.UpdateOrganization;

public record UpdateOrganizationCommand : IRequest<Result<OrganizationDto>>
{
    public Guid OrganizationId { get; init; }

    public UpdateOrganizationRequest Request { get; init; } = null!;
}
