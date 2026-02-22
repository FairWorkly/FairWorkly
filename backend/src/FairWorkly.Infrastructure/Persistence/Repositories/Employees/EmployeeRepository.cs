using FairWorkly.Application.Employees.Interfaces;
using FairWorkly.Domain.Employees.Entities;
using Microsoft.EntityFrameworkCore;

namespace FairWorkly.Infrastructure.Persistence.Repositories.Employees;

/// <summary>
/// Repository implementation for Employee entity.
/// Add() only tracks the entity — SaveChanges is called by the Handler via IUnitOfWork.
/// Updates are handled by EF Core change tracking automatically.
/// </summary>
public class EmployeeRepository : IEmployeeRepository
{
    private readonly FairWorklyDbContext _context;

    public EmployeeRepository(FairWorklyDbContext context)
    {
        _context = context;
    }

    public async Task<List<Employee>> GetByEmployeeNumbersAsync(
        Guid organizationId,
        List<string> employeeNumbers,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .Employees.Where(e =>
                e.OrganizationId == organizationId && employeeNumbers.Contains(e.EmployeeNumber!)
            )
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Employee>> GetByEmailsAsync(
        Guid organizationId,
        List<string> emails,
        CancellationToken cancellationToken = default
    )
    {
        // Convert emails to lowercase for case-insensitive comparison
        var lowerEmails = emails.Select(e => e.ToLowerInvariant()).ToList();

        return await _context
            .Employees.Where(e =>
                e.OrganizationId == organizationId
                && e.Email != null
                && lowerEmails.Contains(e.Email.ToLower())
            )
            .ToListAsync(cancellationToken);
    }

    public void Add(Employee employee)
    {
        _context.Employees.Add(employee);
    }
}
