using FairWorkly.Application.Common.Interfaces;

namespace FairWorkly.Application.Payroll.Orchestrators;

public class PayrollAiOrchestrator
{
    private readonly IAiClient _aiClient;

    public PayrollAiOrchestrator(IAiClient aiClient)
    {
        _aiClient = aiClient;
    }
}
