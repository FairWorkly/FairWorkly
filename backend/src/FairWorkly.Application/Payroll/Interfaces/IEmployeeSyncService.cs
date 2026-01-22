using FairWorkly.Application.Payroll.Services.Models;

namespace FairWorkly.Application.Payroll.Interfaces;

/// <summary>
/// Service for synchronizing employee data from CSV to database
/// </summary>
public interface IEmployeeSyncService
{
    /// <summary>
    /// Synchronizes employees from CSV rows to the database (Upsert operation)
    /// Creates new employees if they don't exist, updates existing ones
    /// </summary>
    /// <param name="rows">Parsed CSV rows containing employee data</param>
    /// <param name="organizationId">Organization ID to associate employees with</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Dictionary mapping EmployeeNumber (from CSV) to Employee database Id
    /// This mapping is used by subsequent payroll processing steps
    /// </returns>
    /// <remarks>
    /// Upsert logic:
    /// - Query existing employees by (EmployeeNumber + OrganizationId)
    /// - If exists: Update FirstName, LastName, AwardType, AwardLevelNumber, EmploymentType
    /// - If not exists: Create new employee with data from CSV
    /// - Return mapping for all employees processed
    /// </remarks>
    Task<Dictionary<string, Guid>> SyncEmployeesAsync(
        List<PayrollCsvRow> rows,
        Guid organizationId,
        CancellationToken cancellationToken = default);
}
