using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Employees.Interfaces;
using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Application.Payroll.Services.Models;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Employees.Entities;

namespace FairWorkly.Application.Payroll.Services;

/// <summary>
/// Service for synchronizing employee data from CSV to database
/// Performs Upsert operations (Create or Update)
/// </summary>
public class EmployeeSyncService : IEmployeeSyncService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public EmployeeSyncService(
        IEmployeeRepository employeeRepository,
        IDateTimeProvider dateTimeProvider
    )
    {
        _employeeRepository = employeeRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Dictionary<string, Guid>> SyncEmployeesAsync(
        List<PayrollCsvRow> rows,
        Guid organizationId,
        CancellationToken cancellationToken = default
    )
    {
        // Early return for null or empty input
        if (rows == null || rows.Count == 0)
            return new Dictionary<string, Guid>();

        var employeeMapping = new Dictionary<string, Guid>();

        // Get unique employee numbers from CSV
        var validRows = rows.Where(r => !string.IsNullOrWhiteSpace(r.EmployeeId)).ToList();
        var employeeNumbers = validRows.Select(r => r.EmployeeId).Distinct().ToList();

        // Fetch existing employees by organization and employee numbers
        var existingEmployees = await _employeeRepository.GetByEmployeeNumbersAsync(
            organizationId,
            employeeNumbers,
            cancellationToken
        );

        // Filter out any employees with null EmployeeNumber (defensive against dirty data)
        var existingEmployeesDict = existingEmployees
            .Where(e => e.EmployeeNumber != null)
            .ToDictionary(e => e.EmployeeNumber!);

        // Group rows by EmployeeId to get the latest data for each employee
        var employeeGroups = validRows.GroupBy(r => r.EmployeeId).ToList();

        foreach (var group in employeeGroups)
        {
            var employeeNumber = group.Key;
            // Take most recent pay period data
            var latestRow = group.OrderByDescending(r => r.PayPeriodEnd).First();

            // Parse employee data from CSV row
            var (firstName, lastName) = ParseEmployeeName(latestRow.EmployeeName);
            var awardType = ParseAwardType(latestRow.AwardType);
            var awardLevelNumber = ParseAwardLevel(latestRow.Classification);
            var employmentType = ParseEmploymentType(latestRow.EmploymentType);

            Employee employee;

            if (existingEmployeesDict.TryGetValue(employeeNumber, out var existingEmployee))
            {
                // Update existing employee
                existingEmployee.FirstName = firstName;
                existingEmployee.LastName = lastName;
                existingEmployee.AwardType = awardType;
                existingEmployee.AwardLevelNumber = awardLevelNumber;
                existingEmployee.EmploymentType = employmentType;

                await _employeeRepository.UpdateAsync(existingEmployee, cancellationToken);
                employee = existingEmployee;
            }
            else
            {
                // Create new employee
                employee = new Employee
                {
                    OrganizationId = organizationId,
                    EmployeeNumber = employeeNumber,
                    FirstName = firstName,
                    LastName = lastName,
                    // Use a generated email for MVP (required field)
                    Email = GeneratePlaceholderEmail(employeeNumber),
                    JobTitle = "Employee", // Default value for MVP
                    AwardType = awardType,
                    AwardLevelNumber = awardLevelNumber,
                    EmploymentType = employmentType,
                    StartDate = _dateTimeProvider.UtcNow.UtcDateTime,
                    IsActive = true,
                };

                await _employeeRepository.CreateAsync(employee, cancellationToken);
            }

            employeeMapping[employeeNumber] = employee.Id;
        }

        return employeeMapping;
    }

    /// <summary>
    /// Parses employee name into first name and last name
    /// Example: "Alice Johnson" -> ("Alice", "Johnson")
    /// </summary>
    private (string FirstName, string LastName) ParseEmployeeName(string fullName)
    {
        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0)
            return ("Unknown", "Unknown");

        if (parts.Length == 1)
            return (parts[0], parts[0]);

        // First part is first name, rest is last name
        var firstName = parts[0];
        var lastName = string.Join(" ", parts.Skip(1));

        return (firstName, lastName);
    }

    /// <summary>
    /// Parses award type from CSV string
    /// Handles: "Retail", "MA000004", "Retail Award", etc.
    /// </summary>
    private AwardType ParseAwardType(string awardTypeString)
    {
        var normalized = awardTypeString.ToLowerInvariant().Trim();

        if (normalized.Contains("retail") || normalized.Contains("ma000004"))
            return AwardType.GeneralRetailIndustryAward2020;

        if (normalized.Contains("hospitality") || normalized.Contains("ma000009"))
            return AwardType.HospitalityIndustryAward2020;

        if (normalized.Contains("clerk") || normalized.Contains("ma000002"))
            return AwardType.ClerksPrivateSectorAward2020;

        // Default to Retail for MVP
        return AwardType.GeneralRetailIndustryAward2020;
    }

    /// <summary>
    /// Parses award level from classification string
    /// Example: "Level 1" -> 1, "Level 2" -> 2
    /// </summary>
    private int ParseAwardLevel(string classification)
    {
        var normalized = classification.Trim().ToLowerInvariant();

        // Try to extract number from "Level X" format
        if (normalized.StartsWith("level"))
        {
            var parts = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2 && int.TryParse(parts[1], out var level))
            {
                return level;
            }
        }

        // Try to parse as direct number
        if (int.TryParse(normalized, out var directLevel))
        {
            return directLevel;
        }

        // Default to Level 1 if cannot parse
        return 1;
    }

    /// <summary>
    /// Parses employment type from CSV string
    /// </summary>
    private EmploymentType ParseEmploymentType(string employmentTypeString)
    {
        var normalized = employmentTypeString.Trim().ToLowerInvariant();

        return normalized switch
        {
            "fulltime" or "full-time" or "full time" => EmploymentType.FullTime,
            "parttime" or "part-time" or "part time" => EmploymentType.PartTime,
            "casual" => EmploymentType.Casual,
            "fixedterm" or "fixed-term" or "fixed term" => EmploymentType.FixedTerm,
            _ => EmploymentType.FullTime, // Default to FullTime
        };
    }

    /// <summary>
    /// Generates a placeholder email for MVP
    /// Sanitizes employee number to ensure valid email format
    /// </summary>
    private string GeneratePlaceholderEmail(string employeeNumber)
    {
        var sanitized = new string(
            employeeNumber.Where(c => char.IsLetterOrDigit(c)).ToArray()
        ).ToLower();

        if (string.IsNullOrEmpty(sanitized))
            sanitized = Guid.NewGuid().ToString("N")[..8];

        return $"{sanitized}@placeholder.local";
    }
}
