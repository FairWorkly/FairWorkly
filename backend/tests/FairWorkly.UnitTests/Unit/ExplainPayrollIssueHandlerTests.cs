using System.Net;
using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Payroll.Features.ExplainIssue;
using FairWorkly.Application.Payroll.Features.ExplainIssue.Dtos;
using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Domain.Payroll.Entities;
using FairWorkly.Domain.Payroll.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Refit;

namespace FairWorkly.UnitTests.Unit;

public class ExplainPayrollIssueHandlerTests
{
    private readonly Mock<IPayrollAgentService> _agentServiceMock = new();
    private readonly Mock<IPayrollIssueRepository> _issueRepoMock = new();
    private readonly Mock<ICurrentUserService> _currentUserMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ILogger<ExplainPayrollIssueHandler>> _loggerMock = new();
    private readonly ExplainPayrollIssueHandler _handler;

    private readonly Guid _orgId = Guid.NewGuid();
    private readonly Guid _issueId = Guid.NewGuid();

    public ExplainPayrollIssueHandlerTests()
    {
        _handler = new ExplainPayrollIssueHandler(
            _agentServiceMock.Object,
            _issueRepoMock.Object,
            _currentUserMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object
        );
    }

    private static ExplainPayrollIssueCommand CreateValidCommand(Guid issueId) =>
        new()
        {
            IssueId = issueId,
            CategoryType = "PenaltyRate",
            EmployeeName = "Alice Smith",
            EmployeeId = "E001",
            Severity = 3,
            ImpactAmount = 31.6m,
            Description = new ExplainIssueDescriptionInput
            {
                ActualValue = 30m,
                ExpectedValue = 33.95m,
                AffectedUnits = 8m,
                UnitType = "Hour",
                ContextLabel = "Saturday (125% rate)",
            },
        };

    private PayrollIssue CreateNormalIssue() =>
        new()
        {
            Id = _issueId,
            OrganizationId = _orgId,
            PayrollValidationId = Guid.NewGuid(),
            PayslipId = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            CategoryType = IssueCategory.PenaltyRate,
            Severity = Domain.Common.Enums.IssueSeverity.Error,
            WarningMessage = null,
        };

    private PayrollIssue CreateWarningIssue() =>
        new()
        {
            Id = _issueId,
            OrganizationId = _orgId,
            PayrollValidationId = Guid.NewGuid(),
            PayslipId = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            CategoryType = IssueCategory.BaseRate,
            Severity = Domain.Common.Enums.IssueSeverity.Warning,
            WarningMessage = "Negative Ordinary Pay detected. Skipping compliance check.",
        };

    private static ApiResponse<AgentExplainResponse> CreateSuccessResponse() =>
        new(
            new HttpResponseMessage(HttpStatusCode.OK),
            new AgentExplainResponse
            {
                Code = 200,
                Msg = "OK",
                Data = new AgentExplainData
                {
                    Type = "payroll_explain",
                    DetailedExplanation = "Saturday hours must be paid at 125%.",
                    Recommendation = "Correct the Saturday rate to $33.95/hr.",
                    Model = "gpt-4o-mini",
                    Sources =
                    [
                        new AgentSourceItem
                        {
                            Source = "AWARD.pdf",
                            Page = 29,
                            Content = "Saturday work must be paid at 125%...",
                        },
                    ],
                },
            },
            new RefitSettings()
        );

    private void SetupUserWithOrg()
    {
        _currentUserMock.Setup(u => u.OrganizationId).Returns(_orgId);
    }

    private void SetupIssueFound(PayrollIssue issue)
    {
        _issueRepoMock
            .Setup(r => r.GetByIdAsync(_issueId, _orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(issue);
    }

    // ==================== Guard branches ====================

    [Fact]
    public async Task Handle_UserWithoutOrganization_Returns403()
    {
        _currentUserMock.Setup(u => u.OrganizationId).Returns((Guid?)null);
        var command = CreateValidCommand(_issueId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Code.Should().Be(403);
    }

    [Fact]
    public async Task Handle_IssueNotFound_Returns404()
    {
        SetupUserWithOrg();
        _issueRepoMock
            .Setup(r => r.GetByIdAsync(_issueId, _orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PayrollIssue?)null);
        var command = CreateValidCommand(_issueId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Code.Should().Be(404);
    }

    // ==================== Warning defense ====================

    [Fact]
    public async Task Handle_WarningIssue_Returns200WithWarning_DoesNotCallAi()
    {
        SetupUserWithOrg();
        SetupIssueFound(CreateWarningIssue());
        var command = CreateValidCommand(_issueId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Code.Should().Be(200);
        result
            .Value!.Warning.Should()
            .Be("Negative Ordinary Pay detected. Skipping compliance check.");
        result.Value.DetailedExplanation.Should().BeNull();
        result.Value.Recommendation.Should().BeNull();

        _agentServiceMock.Verify(
            s =>
                s.ExplainIssueAsync(
                    It.IsAny<PayrollExplainRequest>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
    }

    // ==================== AI success ====================

    [Fact]
    public async Task Handle_AiSuccess_Returns200WithDto()
    {
        SetupUserWithOrg();
        SetupIssueFound(CreateNormalIssue());
        _agentServiceMock
            .Setup(s =>
                s.ExplainIssueAsync(
                    It.IsAny<PayrollExplainRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(CreateSuccessResponse());
        var command = CreateValidCommand(_issueId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Code.Should().Be(200);
        result.Value!.IssueId.Should().Be(_issueId);
        result.Value.DetailedExplanation.Should().Be("Saturday hours must be paid at 125%.");
        result.Value.Recommendation.Should().Be("Correct the Saturday rate to $33.95/hr.");
        result.Value.Model.Should().Be("gpt-4o-mini");
    }

    [Fact]
    public async Task Handle_AiSuccess_UpdatesDbAndSaves()
    {
        SetupUserWithOrg();
        var issue = CreateNormalIssue();
        SetupIssueFound(issue);
        _agentServiceMock
            .Setup(s =>
                s.ExplainIssueAsync(
                    It.IsAny<PayrollExplainRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(CreateSuccessResponse());
        var command = CreateValidCommand(_issueId);

        await _handler.Handle(command, CancellationToken.None);

        issue.DetailedExplanation.Should().Be("Saturday hours must be paid at 125%.");
        issue.Recommendation.Should().Be("Correct the Saturday rate to $33.95/hr.");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AiSuccess_SourcesPassedThrough()
    {
        SetupUserWithOrg();
        SetupIssueFound(CreateNormalIssue());
        _agentServiceMock
            .Setup(s =>
                s.ExplainIssueAsync(
                    It.IsAny<PayrollExplainRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(CreateSuccessResponse());
        var command = CreateValidCommand(_issueId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Value!.Sources.Should().HaveCount(1);
        result.Value.Sources[0].Source.Should().Be("AWARD.pdf");
        result.Value.Sources[0].Page.Should().Be(29);
        result.Value.Sources[0].Content.Should().Be("Saturday work must be paid at 125%...");
    }

    // ==================== AI failure → 503 ====================

    [Fact]
    public async Task Handle_AiReturnsHttp503_Returns503_DbNotUpdated()
    {
        SetupUserWithOrg();
        SetupIssueFound(CreateNormalIssue());
        _agentServiceMock
            .Setup(s =>
                s.ExplainIssueAsync(
                    It.IsAny<PayrollExplainRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new ApiResponse<AgentExplainResponse>(
                    new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
                    default,
                    new RefitSettings()
                )
            );
        var command = CreateValidCommand(_issueId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Code.Should().Be(503);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_AiReturnsEnvelopeCodeNon200_Returns503()
    {
        SetupUserWithOrg();
        SetupIssueFound(CreateNormalIssue());
        _agentServiceMock
            .Setup(s =>
                s.ExplainIssueAsync(
                    It.IsAny<PayrollExplainRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new ApiResponse<AgentExplainResponse>(
                    new HttpResponseMessage(HttpStatusCode.OK),
                    new AgentExplainResponse
                    {
                        Code = 500,
                        Msg = "error",
                        Data = null,
                    },
                    new RefitSettings()
                )
            );
        var command = CreateValidCommand(_issueId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Code.Should().Be(503);
    }

    [Fact]
    public async Task Handle_AiReturnsDataNull_Returns503()
    {
        SetupUserWithOrg();
        SetupIssueFound(CreateNormalIssue());
        _agentServiceMock
            .Setup(s =>
                s.ExplainIssueAsync(
                    It.IsAny<PayrollExplainRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new ApiResponse<AgentExplainResponse>(
                    new HttpResponseMessage(HttpStatusCode.OK),
                    new AgentExplainResponse
                    {
                        Code = 200,
                        Msg = "OK",
                        Data = null,
                    },
                    new RefitSettings()
                )
            );
        var command = CreateValidCommand(_issueId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Code.Should().Be(503);
    }
}
