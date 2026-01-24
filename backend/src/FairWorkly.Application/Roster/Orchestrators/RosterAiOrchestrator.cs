using FairWorkly.Application.Common.Interfaces;

namespace FairWorkly.Application.Roster.Orchestrators;

public class RosterAiOrchestrator(IAiClient aiClient)
{
    private readonly IAiClient _aiClient = aiClient;
}
