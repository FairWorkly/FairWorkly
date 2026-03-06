using System.Diagnostics;
using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Payroll.Features.ExplainIssue.Dtos;
using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Common.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using Refit;

namespace FairWorkly.Application.Payroll.Features.ExplainIssue;

public class ExplainPayrollIssueHandler(
    IPayrollAgentService payrollAgentService,
    IPayrollIssueRepository issueRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork,
    ILogger<ExplainPayrollIssueHandler> logger
) : IRequestHandler<ExplainPayrollIssueCommand, Result<ExplainPayrollIssueDto>>
{
    /// <summary>
    /// Per-request timeout (seconds). Independent from HttpClient global 120s ceiling.
    /// Different Features can set different timeouts (e.g., BatchExplain could use 90s).
    /// </summary>
    private const int RequestTimeoutSeconds = 25;

    public async Task<Result<ExplainPayrollIssueDto>> Handle(
        ExplainPayrollIssueCommand command,
        CancellationToken cancellationToken
    )
    {
        var stopwatch = Stopwatch.StartNew();

        // ═══ Guard: user must belong to an organization ═══
        var organizationId = currentUserService.OrganizationId;
        if (organizationId == null)
            return Result<ExplainPayrollIssueDto>.Of403("User does not belong to an organization");

        var orgId = organizationId.Value;

        logger.LogInformation(
            "ExplainIssue request received: issueId={IssueId}, category={Category}",
            command.IssueId,
            command.CategoryType
        );

        // ═══ DB lookup (security: organization scoping) ═══
        var issue = await issueRepository.GetByIdAsync(command.IssueId, orgId, cancellationToken);

        if (issue == null)
        {
            logger.LogWarning(
                "ExplainIssue issue not found: issueId={IssueId}, orgId={OrgId}",
                command.IssueId,
                orgId
            );
            return Result<ExplainPayrollIssueDto>.Of404("Issue not found");
        }

        // ═══ Warning defense: warning issues skip AI ═══
        if (issue.WarningMessage != null)
        {
            logger.LogInformation(
                "ExplainIssue skipped (warning issue): issueId={IssueId}",
                command.IssueId
            );

            var warningDto = new ExplainPayrollIssueDto
            {
                IssueId = command.IssueId,
                DetailedExplanation = null,
                Recommendation = null,
                Warning = issue.WarningMessage,
            };
            return Result<ExplainPayrollIssueDto>.Of200(
                "Warning issue — no AI explanation needed",
                warningDto
            );
        }

        // ═══ Build Refit request body ═══
        var request = new PayrollExplainRequest
        {
            IssueId = command.IssueId,
            CategoryType = command.CategoryType,
            EmployeeName = command.EmployeeName,
            EmployeeId = command.EmployeeId,
            Severity = ((IssueSeverity)command.Severity).ToString(),
            ImpactAmount = command.ImpactAmount,
            Description =
                command.Description != null
                    ? new PayrollExplainDescription
                    {
                        ActualValue = command.Description.ActualValue,
                        ExpectedValue = command.Description.ExpectedValue,
                        AffectedUnits = command.Description.AffectedUnits,
                        UnitType = command.Description.UnitType,
                        ContextLabel = command.Description.ContextLabel,
                    }
                    : null,
            Warning = command.Warning,
        };

        // ═══ Call AI (per-request timeout) ═══
        using var timeoutCts = new CancellationTokenSource(
            TimeSpan.FromSeconds(RequestTimeoutSeconds)
        );
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            timeoutCts.Token
        );

        ApiResponse<AgentExplainResponse> response;
        try
        {
            response = await payrollAgentService.ExplainIssueAsync(request, linkedCts.Token);
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            // Business timeout (25s) — return clean 503.
            // User-initiated cancellation (browser close) does NOT match this filter,
            // so it propagates normally.
            stopwatch.Stop();
            logger.LogWarning(
                "ExplainIssue AI timed out: issueId={IssueId}, elapsedMs={ElapsedMs}",
                command.IssueId,
                stopwatch.ElapsedMilliseconds
            );
            return Result<ExplainPayrollIssueDto>.Of503(
                "AI service timed out. Please try again later."
            );
        }

        stopwatch.Stop();

        // ═══ Check Refit response (three defenses: HTTP 2xx → envelope code 200 → Data non-null) ═══
        if (
            response.IsSuccessful
            && response.Content?.Code == 200
            && response.Content?.Data is { } data
        )
        {
            logger.LogInformation(
                "ExplainIssue AI succeeded: issueId={IssueId}, model={Model}, elapsedMs={ElapsedMs}",
                command.IssueId,
                data.Model,
                stopwatch.ElapsedMilliseconds
            );

            // Persist to DB (archival, not cache)
            issue.DetailedExplanation = data.DetailedExplanation;
            issue.Recommendation = data.Recommendation;
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Build return DTO
            var dto = new ExplainPayrollIssueDto
            {
                IssueId = command.IssueId,
                DetailedExplanation = data.DetailedExplanation,
                Recommendation = data.Recommendation,
                Model = data.Model,
                Sources =
                    data.Sources?.Select(s => new ExplainSourceDto
                        {
                            Source = s.Source,
                            Page = s.Page,
                            Content = s.Content,
                        })
                        .ToList() ?? [],
            };
            return Result<ExplainPayrollIssueDto>.Of200("AI explanation generated", dto);
        }

        // ═══ Failure: unified 503 ═══
        // Python 400/500/502/503 all map to .NET 503.
        // Frontend does not need to distinguish "FAISS down" vs "LLM timeout".
        var statusCode = response.StatusCode;
        var errorContent = response.Error?.Content;
        logger.LogWarning(
            "ExplainIssue AI failed: issueId={IssueId}, statusCode={StatusCode}, error={Error}, elapsedMs={ElapsedMs}",
            command.IssueId,
            (int)statusCode,
            errorContent,
            stopwatch.ElapsedMilliseconds
        );

        return Result<ExplainPayrollIssueDto>.Of503(
            "AI service is temporarily unavailable. Please try again later."
        );
    }
}
