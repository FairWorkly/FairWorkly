using System.Globalization;
using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Employees.Interfaces;
using FairWorkly.Application.Roster.Interfaces;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Result;
using FairWorkly.Domain.Roster.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FairWorkly.Application.Roster.Features.UploadRoster;

/// <summary>
/// Handles roster file upload by:
/// 1. Sending file to Agent Service for parsing
/// 2. Validating parsed result (blocking errors, dates, entries)
/// 3. Uploading file to S3 for audit trail (only after successful parse)
/// 4. Matching parsed entries to existing employees
/// 5. Creating Roster and Shift entities
/// 6. Saving to database in single transaction
/// 7. Returning response with non-blocking warnings (reserved; usually empty under current policy)
/// </summary>
public class UploadRosterHandler(
    IAiClient aiClient,
    IEmployeeRepository employeeRepository,
    IRosterRepository rosterRepository,
    IFileStorageService fileStorageService,
    IUnitOfWork unitOfWork,
    ILogger<UploadRosterHandler> logger
) : IRequestHandler<UploadRosterCommand, Result<UploadRosterResponse>>
{
    public async Task<Result<UploadRosterResponse>> Handle(
        UploadRosterCommand request,
        CancellationToken cancellationToken
    )
    {
        // Snapshot file bytes — both Agent Service and S3 need to read the stream,
        // but HttpClient disposes the stream after sending, so each consumer
        // gets its own MemoryStream from the shared byte array.
        byte[] fileBytes;
        using (var buffer = new MemoryStream())
        {
            await request.FileStream.CopyToAsync(buffer, cancellationToken);
            fileBytes = buffer.ToArray();
        }

        // ========== Step 1: Parse file via Agent Service ==========
        ParseResponse parseResponse;
        try
        {
            using var agentStream = new MemoryStream(fileBytes);
            var agentResponse = await aiClient.PostMultipartAsync<AgentChatResponse>(
                "/api/agent/chat",
                agentStream,
                request.FileName,
                request.ContentType,
                "Parse this roster file",
                cancellationToken
            );

            // Extract ParseResponse from wrapper
            parseResponse = agentResponse.Result;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to parse roster file via Agent Service. File: {FileName}",
                request.FileName
            );
            return Result<UploadRosterResponse>.Of500(
                "Failed to parse roster file. Please try again or contact support."
            );
        }

        // Guard against unexpected response shape (e.g. Agent Service routed to wrong feature)
        if (parseResponse?.Summary == null || parseResponse.Result == null)
        {
            return Result<UploadRosterResponse>.Of422(
                "Agent Service returned an unexpected response format. Ensure the file was routed to the roster parser."
            );
        }

        // Normalise Issues to avoid null checks downstream
        parseResponse.Issues ??= [];

        // ========== Step 2: Check for blocking errors ==========
        if (parseResponse.Summary.Status == "blocking" || parseResponse.Summary.ErrorCount > 0)
        {
            var errorMessages = parseResponse
                .Issues.Where(i => i.Severity == "error")
                .Select(i => $"Row {i.Row}: {i.Message}")
                .ToList();

            return Result<UploadRosterResponse>.Of422(
                $"Roster file contains errors that prevent import:\n{string.Join("\n", errorMessages)}"
            );
        }

        // ========== Step 3: Validate parsed result has data and valid dates ==========
        if (parseResponse.Result.Entries.Count == 0)
        {
            return Result<UploadRosterResponse>.Of422(
                "Roster file contains no valid shift entries"
            );
        }

        if (
            !parseResponse.Result.WeekStartDate.HasValue
            || !parseResponse.Result.WeekEndDate.HasValue
        )
        {
            return Result<UploadRosterResponse>.Of422("Could not determine week dates from roster");
        }

        // Agent Service returns dates as ISO strings which deserialize as DateTimeKind.Unspecified.
        // PostgreSQL timestamp with time zone requires UTC.
        var weekStart = DateTime.SpecifyKind(
            parseResponse.Result.WeekStartDate.Value,
            DateTimeKind.Utc
        );
        var weekEnd = DateTime.SpecifyKind(
            parseResponse.Result.WeekEndDate.Value,
            DateTimeKind.Utc
        );

        // Validate dates are reasonable (within 2 years of now)
        var now = DateTime.UtcNow;
        if (weekStart < now.AddYears(-2) || weekStart > now.AddYears(2))
        {
            return Result<UploadRosterResponse>.Of422(
                "Roster dates are outside acceptable range (must be within 2 years of today)"
            );
        }

        // ========== Step 4: Upload file to S3 (only after successful parse + validation) ==========
        string s3Key;
        try
        {
            using var s3Stream = new MemoryStream(fileBytes);
            s3Key = await fileStorageService.UploadAsync(
                s3Stream,
                request.FileName,
                cancellationToken
            );
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to upload roster file to storage. File: {FileName}",
                request.FileName
            );
            return Result<UploadRosterResponse>.Of500(
                "Failed to store roster file. Please try again or contact support."
            );
        }

        // ========== Steps 5–8 wrapped to clean up S3 on failure ==========
        try
        {
            return await CreateRosterAndShifts(
                parseResponse,
                request,
                s3Key,
                weekStart,
                weekEnd,
                cancellationToken
            );
        }
        catch (OperationCanceledException)
        {
            // Client disconnected — clean up orphaned S3 file, use CancellationToken.None
            // since the original token is already cancelled
            try
            {
                await fileStorageService.DeleteAsync(s3Key, CancellationToken.None);
            }
            catch (Exception deleteEx)
            {
                logger.LogWarning(deleteEx, "Failed to delete orphaned S3 file: {S3Key}", s3Key);
            }
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to create roster after S3 upload. Cleaning up file: {S3Key}",
                s3Key
            );
            try
            {
                await fileStorageService.DeleteAsync(s3Key, CancellationToken.None);
            }
            catch (Exception deleteEx)
            {
                logger.LogWarning(deleteEx, "Failed to delete orphaned S3 file: {S3Key}", s3Key);
            }

            return Result<UploadRosterResponse>.Of500(
                "Failed to save roster. Please try again or contact support."
            );
        }
    }

    private async Task<Result<UploadRosterResponse>> CreateRosterAndShifts(
        ParseResponse parseResponse,
        UploadRosterCommand request,
        string s3Key,
        DateTime weekStart,
        DateTime weekEnd,
        CancellationToken cancellationToken
    )
    {
        // ========== Step 5: Bulk lookup employees by employee_number and email ==========
        var employeeNumbers = parseResponse
            .Result.Entries.Where(e => !string.IsNullOrWhiteSpace(e.EmployeeNumber))
            .Select(e => e.EmployeeNumber!)
            .Distinct()
            .ToList();

        var emails = parseResponse
            .Result.Entries.Where(e => !string.IsNullOrWhiteSpace(e.EmployeeEmail))
            .Select(e => e.EmployeeEmail!)
            .Distinct()
            .ToList();

        var employeesByNumber = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        var employeesByEmail = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

        if (employeeNumbers.Any())
        {
            var matchedByNumber = await employeeRepository.GetByEmployeeNumbersAsync(
                request.OrganizationId,
                employeeNumbers,
                cancellationToken
            );

            foreach (var emp in matchedByNumber)
            {
                if (!string.IsNullOrWhiteSpace(emp.EmployeeNumber))
                {
                    employeesByNumber[emp.EmployeeNumber] = emp.Id;
                }
            }
        }

        if (emails.Any())
        {
            var matchedByEmail = await employeeRepository.GetByEmailsAsync(
                request.OrganizationId,
                emails,
                cancellationToken
            );

            foreach (var emp in matchedByEmail)
            {
                if (!string.IsNullOrWhiteSpace(emp.Email))
                {
                    employeesByEmail[emp.Email] = emp.Id;
                }
            }
        }

        // ========== Step 6: Create Roster entity ==========
        var roster = new Domain.Roster.Entities.Roster
        {
            Id = Guid.NewGuid(),
            OrganizationId = request.OrganizationId,
            WeekStartDate = weekStart,
            WeekEndDate = weekEnd,
            WeekNumber = ISOWeek.GetWeekOfYear(weekStart),
            Year = weekStart.Year,
            IsFinalized = false,
            TotalShifts = parseResponse.Result.TotalShifts,
            TotalHours = parseResponse.Result.TotalHours,
            TotalEmployees = parseResponse.Result.UniqueEmployees,
            OriginalFileS3Key = s3Key,
            OriginalFileName = request.FileName,
            Notes = BuildRosterNotes(parseResponse),
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedByUserId = request.UserId,
        };

        await rosterRepository.CreateAsync(roster, cancellationToken);

        // ========== Step 7: Create Shift entities, matching to employees ==========
        var shifts = new List<Shift>();
        var unmatchedEmployees = new List<string>();
        var totalNetHours = 0m;

        foreach (var entry in parseResponse.Result.Entries)
        {
            Guid employeeId = Guid.Empty;

            // Try match by employee_number first
            if (
                !string.IsNullOrWhiteSpace(entry.EmployeeNumber)
                && employeesByNumber.TryGetValue(entry.EmployeeNumber, out var empIdByNumber)
            )
            {
                employeeId = empIdByNumber;
            }
            // Fall back to email
            else if (
                !string.IsNullOrWhiteSpace(entry.EmployeeEmail)
                && employeesByEmail.TryGetValue(entry.EmployeeEmail, out var empIdByEmail)
            )
            {
                employeeId = empIdByEmail;
            }
            else
            {
                // No match found - skip shift creation, track for warning
                var identifier =
                    entry.EmployeeNumber ?? entry.EmployeeEmail ?? entry.EmployeeName ?? "Unknown";
                if (!unmatchedEmployees.Contains(identifier))
                {
                    unmatchedEmployees.Add(identifier);
                }
                continue;
            }

            var shift = new Shift
            {
                Id = Guid.NewGuid(),
                OrganizationId = request.OrganizationId,
                RosterId = roster.Id,
                EmployeeId = employeeId,
                Date = DateTime.SpecifyKind(entry.Date, DateTimeKind.Utc),
                StartTime = entry.StartTime,
                EndTime = entry.EndTime,
                HasMealBreak = entry.HasMealBreak,
                MealBreakDuration = entry.MealBreakDuration,
                HasRestBreaks = entry.HasRestBreaks,
                RestBreaksDuration = entry.RestBreaksDuration,
                IsPublicHoliday = entry.IsPublicHoliday,
                PublicHolidayName = entry.PublicHolidayName,
                IsOnCall = entry.IsOnCall,
                Location = entry.Location,
                Notes = entry.Notes,
                CreatedAt = DateTimeOffset.UtcNow,
            };

            shifts.Add(shift);
            totalNetHours += entry.NetHours;
        }

        // Update roster stats to reflect actual matched shifts
        roster.TotalShifts = shifts.Count;
        roster.TotalEmployees = shifts.Select(s => s.EmployeeId).Distinct().Count();
        roster.TotalHours = totalNetHours;

        await rosterRepository.CreateShiftsAsync(shifts, cancellationToken);

        // ========== Step 8: Save all changes in single transaction ==========
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // ========== Step 9: Build response with warnings ==========
        // This list is kept for forward compatibility with non-blocking import hints.
        // Current import policy treats most data quality issues as blocking 422 errors.
        var warnings = parseResponse
            .Issues.Where(i => i.Severity == "warning")
            .Select(i => new ParserWarning
            {
                Code = i.Code,
                Message = i.Message,
                Row = i.Row,
                Column = i.Column,
                Value = i.Value,
                Hint = i.Hint,
            })
            .ToList();

        // Add unmatched employee warnings
        if (unmatchedEmployees.Any())
        {
            warnings.Add(
                new ParserWarning
                {
                    Code = "EMPLOYEE_NOT_FOUND",
                    Message =
                        $"{unmatchedEmployees.Count} employee(s) could not be matched to existing employees",
                    Row = 0,
                    Hint =
                        $"Unmatched: {string.Join(", ", unmatchedEmployees.Take(5))}"
                        + (
                            unmatchedEmployees.Count > 5
                                ? $" and {unmatchedEmployees.Count - 5} more"
                                : ""
                        ),
                }
            );
        }

        var response = new UploadRosterResponse
        {
            RosterId = roster.Id,
            WeekStartDate = roster.WeekStartDate,
            WeekEndDate = roster.WeekEndDate,
            TotalShifts = roster.TotalShifts,
            TotalHours = roster.TotalHours,
            TotalEmployees = roster.TotalEmployees,
            Warnings = warnings,
        };

        return Result<UploadRosterResponse>.Of200("Roster uploaded successfully", response);
    }

    /// <summary>
    /// Builds a summary of warnings for Roster.Notes field.
    /// </summary>
    private static string? BuildRosterNotes(ParseResponse parseResponse)
    {
        var warnings = parseResponse.Issues.Where(i => i.Severity == "warning").ToList();
        if (warnings.Count == 0)
        {
            return null;
        }

        var warningGroups = warnings
            .GroupBy(i => i.Code)
            .Select(g => $"{g.Key} ({g.Count()})")
            .ToList();

        return $"Parser warnings: {string.Join(", ", warningGroups)}";
    }
}
