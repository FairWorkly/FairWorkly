using FairWorkly.Application.Roster.Features.ValidateRoster;
using FairWorkly.Application.Roster.Interfaces;
using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.Roster.Features.GetValidationResults;

/// <summary>
/// Handles fetching existing validation results for a roster.
/// Returns 404 if no validation has been run yet.
/// </summary>
public class GetValidationResultsHandler(
    IRosterRepository rosterRepository,
    IRosterValidationRepository validationRepository
) : IRequestHandler<GetValidationResultsQuery, Result<ValidateRosterResponse>>
{
    public async Task<Result<ValidateRosterResponse>> Handle(
        GetValidationResultsQuery request,
        CancellationToken cancellationToken
    )
    {
        var validation = await validationRepository.GetByRosterIdWithIssuesAsync(
            request.RosterId,
            request.OrganizationId,
            cancellationToken
        );

        if (validation == null)
        {
            return Result<ValidateRosterResponse>.Of404("No validation found for this roster");
        }

        if (
            validation.Status == Domain.Common.Enums.ValidationStatus.InProgress
            || ValidationFailureMarker.IsExecutionFailure(validation)
        )
        {
            return Result<ValidateRosterResponse>.Of404(
                "Validation is not currently available. Please trigger validation again."
            );
        }

        // Load roster for employee names/metadata
        var roster = await rosterRepository.GetByIdWithShiftsAsync(
            request.RosterId,
            request.OrganizationId,
            cancellationToken
        );

        if (roster == null)
        {
            return Result<ValidateRosterResponse>.Of404("Roster not found");
        }

        var response = ValidationResponseBuilder.Build(roster, validation, validation.Issues);

        return Result<ValidateRosterResponse>.Of200("Validation results retrieved", response);
    }
}
