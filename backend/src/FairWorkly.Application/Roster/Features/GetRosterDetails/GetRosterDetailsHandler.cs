using FairWorkly.Application.Roster.Interfaces;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.Roster.Features.GetRosterDetails;

/// <summary>
/// Handles fetching roster details with shifts grouped by employee.
/// Used by the results page after roster upload.
/// </summary>
public class GetRosterDetailsHandler(IRosterRepository rosterRepository)
    : IRequestHandler<GetRosterDetailsQuery, Result<RosterDetailsResponse>>
{
    public async Task<Result<RosterDetailsResponse>> Handle(
        GetRosterDetailsQuery request,
        CancellationToken cancellationToken
    )
    {
        var roster = await rosterRepository.GetByIdWithShiftsAsync(
            request.RosterId,
            request.OrganizationId,
            cancellationToken
        );

        if (roster == null)
        {
            return Result<RosterDetailsResponse>.Of404("Roster not found");
        }

        var employeeGroups = roster
            .Shifts.GroupBy(s => s.EmployeeId)
            .Select(g =>
            {
                var employee = g.First().Employee;
                return new EmployeeShiftGroup
                {
                    EmployeeId = g.Key,
                    EmployeeName = employee.FullName,
                    EmployeeNumber = employee.EmployeeNumber,
                    ShiftCount = g.Count(),
                    TotalHours = g.Sum(s => s.Duration),
                    Shifts = g.OrderBy(s => s.Date)
                        .ThenBy(s => s.StartTime)
                        .Select(s => new ShiftSummary
                        {
                            ShiftId = s.Id,
                            Date = s.Date,
                            StartTime = s.StartTime,
                            EndTime = s.EndTime,
                            Duration = s.Duration,
                            HasMealBreak = s.HasMealBreak,
                            MealBreakDuration = s.MealBreakDuration,
                            Location = s.Location,
                        })
                        .ToList(),
                };
            })
            .OrderBy(eg => eg.EmployeeName)
            .ToList();

        var response = new RosterDetailsResponse
        {
            RosterId = roster.Id,
            WeekStartDate = roster.WeekStartDate,
            WeekEndDate = roster.WeekEndDate,
            WeekNumber = roster.WeekNumber,
            Year = roster.Year,
            TotalShifts = roster.TotalShifts,
            TotalHours = roster.TotalHours,
            TotalEmployees = roster.TotalEmployees,
            IsFinalized = roster.IsFinalized,
            OriginalFileName = roster.OriginalFileName,
            CreatedAt = roster.CreatedAt,
            HasValidation = roster.RosterValidation != null,
            ValidationId = roster.RosterValidation?.Id,
            Employees = employeeGroups,
        };

        return Result<RosterDetailsResponse>.Of200("Roster details retrieved", response);
    }
}
