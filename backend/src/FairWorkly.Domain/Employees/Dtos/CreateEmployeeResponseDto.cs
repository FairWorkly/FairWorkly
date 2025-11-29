namespace FairWorkly.Domain.Employees.Dtos;

public class CreateEmployeeResponseDto
{
    public Guid Id { get; set; } // 对应 Entity.Id
    public string FullName { get; set; } = string.Empty; // 需要拼接
    public string Status { get; set; } = string.Empty;

    // AI 生成的欢迎语
    public string AiWelcomeMessage { get; set; } = string.Empty;
}