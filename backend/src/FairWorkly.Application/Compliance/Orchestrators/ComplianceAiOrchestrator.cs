using FairWorkly.Application.Common.Interfaces;

namespace FairWorkly.Application.Compliance.Orchestrators;

public class ComplianceAiOrchestrator
{
    private readonly IAiClient _aiClient;

    public ComplianceAiOrchestrator(IAiClient aiClient)
    {
        _aiClient = aiClient;
    }
}
