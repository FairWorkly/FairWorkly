using FairWorkly.Application.Common.Interfaces;

namespace FairWorkly.Application.Documents.Orchestrators;

public class DocumentAiOrchestrator
{
    private readonly IAiClient _aiClient;

    public DocumentAiOrchestrator(IAiClient aiClient)
    {
        _aiClient = aiClient;
    }
}
