using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Employees.Orchestrators;

// TODO: using FairWorkly.Domain.Employees.Interfaces;

namespace FairWorkly.Application.Employees.Services;

public class EmployeeService : IEmployeeService
{
    // private readonly IEmployeeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAiClient _aiClient;
    private readonly EmployeeAiOrchestrator _orchestrator;

    public EmployeeService(
        // IEmployeeRepository repository,
        IUnitOfWork unitOfWork,
        IAiClient aiClient,
        EmployeeAiOrchestrator orchestrator
    )
    {
        // _repository = repository;
        _unitOfWork = unitOfWork;
        _aiClient = aiClient;
        _orchestrator = orchestrator;
    }
}
