namespace FairWorkly.Application.Compliance.Features.TestArchitecture;

public class TestArchitectureResultDto
{
    public string Status { get; set; } = "Success";

    // 验证 DB 是否工作：返回新创建的员工 ID
    public Guid CreatedEmployeeId { get; set; }
    public string DatabaseCheck { get; set; } = string.Empty;

    // 验证 AI 是否工作：返回 AI 的回复
    public string AiResponse { get; set; } = string.Empty;
    public string AiCheck { get; set; } = string.Empty;
}