using FairWorkly.Application.Payroll.Features.ExplainIssue.Dtos;
using Refit;

namespace FairWorkly.Application.Payroll.Interfaces;

/// <summary>
/// Refit declarative HTTP client: .NET to Python agent-service (Payroll).
/// Refit auto-generates the implementation via Source Generator at compile time.
/// Handlers inject this interface directly through DI.
/// </summary>
public interface IPayrollAgentService
{
    /// <summary>
    /// Call Python agent-service to explain a single payroll compliance issue.
    /// Returns ApiResponse&lt;T&gt; instead of Task&lt;T&gt; so non-2xx responses
    /// do not throw exceptions; callers check IsSuccessful instead.
    /// </summary>
    [Post("/api/agent/payroll/explain")]
    Task<ApiResponse<AgentExplainResponse>> ExplainIssueAsync(
        [Body] PayrollExplainRequest request,
        CancellationToken cancellationToken = default
    );
}
