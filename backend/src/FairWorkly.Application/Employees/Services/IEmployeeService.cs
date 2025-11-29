using FairWorkly.Domain.Employees.Dtos;

namespace FairWorkly.Application.Employees.Services;

public interface IEmployeeService
{
    Task<CreateEmployeeResponseDto> CreateEmployeeAsync(CreateEmployeeRequestDto request, CancellationToken cancellationToken = default);
}