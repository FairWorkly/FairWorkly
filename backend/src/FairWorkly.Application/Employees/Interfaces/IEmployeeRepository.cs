using FairWorkly.Domain.Employees.Entities;

namespace FairWorkly.Application.Employees.Interfaces;

/// <summary>
/// Repository interface for Employee entity operations.
/// Note: Add() only tracks the entity. Handler calls unitOfWork.SaveChangesAsync() at the end.
/// Updates are handled by EF Core change tracking (no explicit Update method needed).
/// </summary>
public interface IEmployeeRepository
{
    /// <summary>
    /// Gets employees by a list of employee numbers within an organization
    /// </summary>
    Task<List<Employee>> GetByEmployeeNumbersAsync(
        Guid organizationId,
        List<string> employeeNumbers,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets employees by a list of email addresses within an organization
    /// </summary>
    /// <param name="organizationId">Organization ID</param>
    /// <param name="emails">List of email addresses to search for (case-insensitive)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching employees</returns>
    Task<List<Employee>> GetByEmailsAsync(
        Guid organizationId,
        List<string> emails,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Adds a new employee to the EF change tracker (does not call SaveChanges)
    /// </summary>
    void Add(Employee employee);
}
