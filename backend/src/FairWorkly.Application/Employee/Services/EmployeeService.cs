using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Employees.Dtos;
using FairWorkly.Domain.Employees.Entities;
using FairWorkly.Domain.Employees.Interfaces;

namespace FairWorkly.Application.Employees.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAiClient _aiClient;
    // ❌ 移除了 IMapper

    public EmployeeService(
        IEmployeeRepository repository,
        IUnitOfWork unitOfWork,
        IAiClient aiClient)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _aiClient = aiClient;
    }

    public async Task<CreateEmployeeResponseDto> CreateEmployeeAsync(CreateEmployeeRequestDto request, CancellationToken cancellationToken = default)
    {
        // 1. 手动映射：DTO -> Entity
        // 这种代码虽然长，但逻辑非常清晰，谁都能看懂
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(), // 模拟租户
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PositionTitle = request.PositionTitle,
            BaseHourlyRate = request.BaseHourlyRate,
            Status = "ACTIVE",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        // 2. 存库
        _repository.Add(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 3. 调用 AI 生成欢迎语
        string aiMessage = "Welcome!";
        try
        {
            var aiRequest = new
            {
                name = $"{employee.FirstName} {employee.LastName}",
                role = employee.PositionTitle
            };

            var aiResponse = await _aiClient.PostAsync<object, AiWelcomeResponseDto>(
                "/employee/welcome",
                aiRequest,
                cancellationToken);

            aiMessage = aiResponse.Reply;
        }
        catch
        {
            aiMessage = "Welcome to the team! (AI unavailable)";
        }

        // 4. 手动映射：Entity -> Response DTO
        return new CreateEmployeeResponseDto
        {
            Id = employee.Id,
            FullName = $"{employee.FirstName} {employee.LastName}",
            Status = employee.Status,
            AiWelcomeMessage = aiMessage
        };
    }

    private class AiWelcomeResponseDto { public string Reply { get; set; } = string.Empty; }
}