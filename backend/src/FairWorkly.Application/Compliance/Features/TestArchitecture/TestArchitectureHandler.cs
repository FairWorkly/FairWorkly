using FairWorkly.Application.Common.Interfaces; // ✅ 引用 IUnitOfWork
using FairWorkly.Application.Compliance.Orchestrators;
using FairWorkly.Domain.Employees.Entities;
using FairWorkly.Domain.Employees.Interfaces;
using MediatR;

namespace FairWorkly.Application.Compliance.Features.TestArchitecture;

public class TestArchitectureHandler : IRequestHandler<TestArchitectureCommand, TestArchitectureResultDto>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork; // ✅ 新增字段
    private readonly ComplianceAiOrchestrator _aiOrchestrator;

    public TestArchitectureHandler(
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork, // ✅ 构造函数注入
        ComplianceAiOrchestrator aiOrchestrator)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _aiOrchestrator = aiOrchestrator;
    }

    public async Task<TestArchitectureResultDto> Handle(TestArchitectureCommand request, CancellationToken cancellationToken)
    {
        var result = new TestArchitectureResultDto();

        // 1. 测试数据库 (Database Infrastructure Test)
        try
        {
            var newEmployee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = request.TestEmployeeName,
                LastName = "TestUser",
                Email = "test@fairworkly.com.au",
                TenantId = Guid.NewGuid(),
                Status = "ACTIVE"
            };

            // 步骤 A: 添加到内存 (Repository)
            _employeeRepository.Add(newEmployee);

            // 步骤 B: 提交到数据库 (UnitOfWork) ✅ 这里的调用是真实的数据库写入操作
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            result.CreatedEmployeeId = newEmployee.Id;
            result.DatabaseCheck = $"✅ Database Entity Persisted Successfully via UnitOfWork. ID: {newEmployee.Id}";
        }
        catch (Exception ex)
        {
            result.DatabaseCheck = $"❌ Database Error: {ex.Message}";
            result.Status = "Partial Failure";
        }

        // ============================================================
        // 2. 测试 AI 链路 (AI Infrastructure Test)
        // ============================================================
        try
        {
            // 调用 Orchestrator -> Client -> (Mock/Real) -> Router -> Agent
            var aiReply = await _aiOrchestrator.ChatWithAiAsync(request.InputMessage);

            result.AiResponse = aiReply;
            result.AiCheck = "✅ AI Pipeline Connected Successfully (Check if response matches Mock/Real)";
        }
        catch (Exception ex)
        {
            result.AiCheck = $"❌ AI Error: {ex.Message}";
            result.Status = "Partial Failure";
        }

        return result;

    }
}