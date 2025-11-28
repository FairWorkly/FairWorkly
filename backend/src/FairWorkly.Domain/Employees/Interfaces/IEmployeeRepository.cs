using FairWorkly.Domain.Employees.Entities;

namespace FairWorkly.Domain.Employees.Interfaces;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Employee employee);
}