using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FairWorkly.Application.Payroll.Features.ExplainIssue.Dtos;
using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Employees.Entities;
using FairWorkly.Domain.Payroll.Entities;
using FairWorkly.Domain.Payroll.Enums;
using FairWorkly.Infrastructure.Persistence;
using FairWorkly.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Refit;

namespace FairWorkly.IntegrationTests.Payroll;

public class ExplainPayrollIssueIntegrationTests : IntegrationTestBase
{
    public ExplainPayrollIssueIntegrationTests(CustomWebApplicationFactory factory)
        : base(factory) { }

    // ═══════════════════════════════════════════════════════════════
    //  Stub: hand-written IPayrollAgentService (no Moq in this project)
    // ═══════════════════════════════════════════════════════════════

    private class StubPayrollAgentService(
        Func<PayrollExplainRequest, CancellationToken, Task<ApiResponse<AgentExplainResponse>>>
            handler
    ) : IPayrollAgentService
    {
        public Task<ApiResponse<AgentExplainResponse>> ExplainIssueAsync(
            PayrollExplainRequest request,
            CancellationToken cancellationToken = default
        ) => handler(request, cancellationToken);
    }

    private static StubPayrollAgentService CreateSuccessStub() =>
        new(
            (_, _) =>
                Task.FromResult(
                    new ApiResponse<AgentExplainResponse>(
                        new HttpResponseMessage(HttpStatusCode.OK),
                        new AgentExplainResponse
                        {
                            Code = 200,
                            Msg = "OK",
                            Data = new AgentExplainData
                            {
                                Type = "payroll_explain",
                                DetailedExplanation =
                                    "Saturday hours must be paid at 125%.",
                                Recommendation =
                                    "Correct the Saturday rate to $33.95/hr.",
                                Model = "gpt-4o-mini",
                                Sources =
                                [
                                    new AgentSourceItem
                                    {
                                        Source = "AWARD.pdf",
                                        Page = 29,
                                        Content = "Saturday work at 125%...",
                                    },
                                ],
                            },
                        },
                        new RefitSettings()
                    )
                )
        );

    private static StubPayrollAgentService CreateFailureStub() =>
        new(
            (_, _) =>
                Task.FromResult(
                    new ApiResponse<AgentExplainResponse>(
                        new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
                        default,
                        new RefitSettings()
                    )
                )
        );

    // ═══════════════════════════════════════════════════════════════
    //  Helpers: DI override + auth + seed
    // ═══════════════════════════════════════════════════════════════

    private async Task<HttpClient> CreateClientWithStub(IPayrollAgentService stub)
    {
        var token = await GetAccessTokenAsync();
        var client = Factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IPayrollAgentService>();
                    services.AddSingleton(stub);
                });
            })
            .CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            token
        );
        return client;
    }

    /// <summary>
    /// Seed a fresh PayrollIssue (+ parent chain) scoped to the test org.
    /// Returns the issue ID for the request body.
    /// </summary>
    private async Task<Guid> SeedTestIssueAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();

        var now = DateTimeOffset.UtcNow;

        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            OrganizationId = TestOrganizationId,
            FirstName = "Test",
            LastName = "Worker",
            EmployeeNumber = $"E{Guid.NewGuid().ToString()[..6]}",
            JobTitle = "Cashier",
            AwardType = AwardType.GeneralRetailIndustryAward2020,
            AwardLevelNumber = 2,
            EmploymentType = EmploymentType.FullTime,
            StartDate = DateTime.SpecifyKind(DateTime.UtcNow.AddYears(-1), DateTimeKind.Utc),
        };

        var validation = new PayrollValidation
        {
            Id = Guid.NewGuid(),
            OrganizationId = TestOrganizationId,
            Status = ValidationStatus.Failed,
            PayPeriodStart = now.AddDays(-14),
            PayPeriodEnd = now,
            TotalPayslips = 1,
            FailedCount = 1,
            TotalIssuesCount = 1,
        };

        var payslip = new Payslip
        {
            Id = Guid.NewGuid(),
            OrganizationId = TestOrganizationId,
            EmployeeId = employee.Id,
            PayrollValidationId = validation.Id,
            PayPeriodStart = now.AddDays(-14),
            PayPeriodEnd = now,
            PayDate = now,
            EmployeeName = "Test Worker",
            EmployeeNumber = employee.EmployeeNumber,
            EmploymentType = EmploymentType.FullTime,
            AwardType = AwardType.GeneralRetailIndustryAward2020,
            Classification = "Level 2",
            HourlyRate = 27.16m,
            OrdinaryHours = 38m,
            OrdinaryPay = 1032.08m,
            GrossPay = 1100m,
            Superannuation = 132m,
        };

        var issue = new PayrollIssue
        {
            Id = Guid.NewGuid(),
            OrganizationId = TestOrganizationId,
            PayrollValidationId = validation.Id,
            PayslipId = payslip.Id,
            EmployeeId = employee.Id,
            CategoryType = IssueCategory.PenaltyRate,
            Severity = IssueSeverity.Error,
            ExpectedValue = 33.95m,
            ActualValue = 30m,
            AffectedUnits = 8m,
            UnitType = "Hour",
            ContextLabel = "Saturday (125% rate)",
        };

        db.Set<Employee>().Add(employee);
        db.Set<PayrollValidation>().Add(validation);
        db.Set<Payslip>().Add(payslip);
        db.PayrollIssues.Add(issue);
        await db.SaveChangesAsync();

        return issue.Id;
    }

    private static object CreateRequestBody(Guid issueId) =>
        new
        {
            issueId,
            categoryType = "PenaltyRate",
            employeeName = "Test Worker",
            employeeId = "E001",
            severity = 3,
            impactAmount = 31.6,
            description = new
            {
                actualValue = 30,
                expectedValue = 33.95,
                affectedUnits = 8,
                unitType = "Hour",
                contextLabel = "Saturday (125% rate)",
            },
        };

    private static JsonElement ParseJson(string json)
    {
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.Clone();
    }

    // ═══════════════════════════════════════════════════════════════
    //  Tests
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task Explain_HappyPath_Returns200WithDto()
    {
        var issueId = await SeedTestIssueAsync();
        var client = await CreateClientWithStub(CreateSuccessStub());

        var response = await client.PostAsJsonAsync("/api/payroll/explain", CreateRequestBody(issueId));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = ParseJson(await response.Content.ReadAsStringAsync());
        json.GetProperty("code").GetInt32().Should().Be(200);

        var data = json.GetProperty("data");
        data.GetProperty("issueId").GetString().Should().Be(issueId.ToString());
        data.GetProperty("detailedExplanation").GetString().Should().NotBeNullOrEmpty();
        data.GetProperty("recommendation").GetString().Should().NotBeNullOrEmpty();
        data.GetProperty("model").GetString().Should().Be("gpt-4o-mini");
        data.GetProperty("sources").GetArrayLength().Should().Be(1);
    }

    [Fact]
    public async Task Explain_Unauthenticated_Returns401()
    {
        var client = Factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/payroll/explain",
            CreateRequestBody(Guid.NewGuid())
        );

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Explain_IssueNotFound_Returns404()
    {
        var randomIssueId = Guid.NewGuid();
        var client = await CreateClientWithStub(CreateSuccessStub());

        var response = await client.PostAsJsonAsync(
            "/api/payroll/explain",
            CreateRequestBody(randomIssueId)
        );

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Explain_AiFailure_Returns503()
    {
        var issueId = await SeedTestIssueAsync();
        var client = await CreateClientWithStub(CreateFailureStub());

        var response = await client.PostAsJsonAsync("/api/payroll/explain", CreateRequestBody(issueId));

        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        var json = ParseJson(await response.Content.ReadAsStringAsync());
        json.GetProperty("code").GetInt32().Should().Be(503);
    }

    [Fact]
    public async Task Explain_HappyPath_PersistsToDb()
    {
        var issueId = await SeedTestIssueAsync();
        var client = await CreateClientWithStub(CreateSuccessStub());

        var response = await client.PostAsJsonAsync("/api/payroll/explain", CreateRequestBody(issueId));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify DB persistence
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
        var dbIssue = await db.PayrollIssues.FirstOrDefaultAsync(i => i.Id == issueId);

        dbIssue.Should().NotBeNull();
        dbIssue!.DetailedExplanation.Should().Be("Saturday hours must be paid at 125%.");
        dbIssue.Recommendation.Should().Be("Correct the Saturday rate to $33.95/hr.");
    }
}
