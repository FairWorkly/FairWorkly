using FairWorkly.Application.Employees.Interfaces;
using FairWorkly.Domain.Employees.Entities;
using Microsoft.EntityFrameworkCore;

namespace FairWorkly.Infrastructure.Persistence.Repositories.Employees;

/// <summary>
/// Repository implementation for Employee entity
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

    public async Task<Employee> CreateAsync(
        Employee employee,
        CancellationToken cancellationToken = default
    )
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync(cancellationToken);
        return employee;
    }

    public async Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
