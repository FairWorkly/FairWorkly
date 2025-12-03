using FairWorkly.Application.Common.Interfaces;

namespace FairWorkly.Application.Employees.Orchestrators;

public class EmployeeAiOrchestrator
{
    private readonly IAiClient _aiClient;

    public EmployeeAiOrchestrator(IAiClient aiClient)
    {
        _aiClient = aiClient;
    }
}
