using FairWorkly.Domain.Employees.Entities;
using FairWorkly.Domain.Employees.Interfaces;
using FairWorkly.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FairWorkly.Infrastructure.Repositories.Employees;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly FairWorklyDbContext _context;

    public EmployeeRepository(FairWorklyDbContext context)
    {
        _context = context;
    }

    public async Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public void Add(Employee employee)
    {
        _context.Employees.Add(employee);
    }
}