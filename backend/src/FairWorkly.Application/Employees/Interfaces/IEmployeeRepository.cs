using FairWorkly.Domain.Employees.Entities;

namespace FairWorkly.Application.Employees.Interfaces;

/// <summary>
/// Repository interface for Employee entity operations
/// </summary>
public interface IEmployeeRepository
{
    /// <summary>
    /// Gets employees by a list of employee numbers within an organization
    /// </summary>
    /// <param name="organizationId">Organization ID</param>
    /// <param name="employeeNumbers">List of employee numbers to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching employees</returns>
    Task<List<Employee>> GetByEmployeeNumbersAsync(
        Guid organizationId,
        List<string> employeeNumbers,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new employee
    /// </summary>
    /// <param name="employee">Employee entity to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created employee with generated Id</returns>
    Task<Employee> CreateAsync(
        Employee employee,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing employee
    /// </summary>
    /// <param name="employee">Employee entity with updated values</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateAsync(
        Employee employee,
        CancellationToken cancellationToken = default);
}
