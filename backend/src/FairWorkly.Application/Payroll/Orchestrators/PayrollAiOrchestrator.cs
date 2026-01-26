using FairWorkly.Application.Common.Interfaces;

namespace FairWorkly.Application.Payroll.Orchestrators
{
    public class PayrollAiOrchestrator(IAiClient aiClient)
    {
        private readonly IAiClient _aiClient = aiClient;
    }
}
