using System.Net;
using System.Text.Json;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Employees.Entities;
using FairWorkly.Infrastructure.Persistence;
using FairWorkly.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FairWorkly.IntegrationTests.Payroll;

public class PayrollValidationTests : IntegrationTestBase
{
    private static readonly string CsvDir = Path.Combine("TestData", "Csv", "Payroll");

    public PayrollValidationTests(CustomWebApplicationFactory factory) : base(factory) { }

    private async Task<HttpResponseMessage> PostValidationAsync(
        string? csvPath = null,
        string awardType = "GeneralRetailIndustryAward2020",
        string state = "VIC",
        bool sendFile = true)
    {
        var client = await CreateAuthenticatedClientAsync();
        using var content = new MultipartFormDataContent();

        if (sendFile && csvPath != null)
        {
            content.Add(new StreamContent(File.OpenRead(csvPath)), "file", Path.GetFileName(csvPath));
        }

        content.Add(new StringContent(awardType), "awardType");
        content.Add(new StringContent(state), "state");

        return await client.PostAsync("/api/payroll/validation", content);
    }

    private static JsonElement ParseJson(string json)
    {
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.Clone();
    }

    // ═══════════════════════════════════════════════════════════════
    //  Case 0: File too large (400 via Validator)
    //  Note: In production Kestrel enforces [RequestSizeLimit] → 413,
    //  but TestServer does not support IHttpMaxRequestBodySizeFeature,
    //  so the request falls through to ValidatePayrollValidator → 400.
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task Post_FileTooLarge_Returns400()
    {
        // Arrange: 3MB payload exceeds 2MB limit
        var client = await CreateAuthenticatedClientAsync();
        using var content = new MultipartFormDataContent();
        var largeBytes = new byte[3 * 1024 * 1024];
        content.Add(new ByteArrayContent(largeBytes), "file", "large.csv");
        content.Add(new StringContent("GeneralRetailIndustryAward2020"), "awardType");
        content.Add(new StringContent("VIC"), "state");

        // Act
        var response = await client.PostAsync("/api/payroll/validation", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var json = ParseJson(await response.Content.ReadAsStringAsync());
        json.GetProperty("code").GetInt32().Should().Be(400);

        var errors = json.GetProperty("data").GetProperty("errors");
        errors.EnumerateArray().Should().Contain(e =>
            e.GetProperty("field").GetString() == "file" &&
            e.GetProperty("message").GetString() == "File size must not exceed 2MB");
    }

    // ═══════════════════════════════════════════════════════════════
    //  Case 1: Layer 1 error format (400)
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task Post_InvalidAwardType_Returns400WithErrorFormat()
    {
        // Arrange: send a valid file but invalid awardType to trigger FluentValidation
        var csvPath = Path.Combine(CsvDir, "compliant.csv");

        // Act
        var response = await PostValidationAsync(csvPath, awardType: "FastFood");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var json = ParseJson(await response.Content.ReadAsStringAsync());
        json.GetProperty("code").GetInt32().Should().Be(400);
        json.GetProperty("msg").GetString().Should().NotBeNullOrEmpty();

        var errors = json.GetProperty("data").GetProperty("errors");
        errors.GetArrayLength().Should().BeGreaterThan(0);

        var firstError = errors[0];
        firstError.GetProperty("field").GetString().Should().NotBeNullOrEmpty();
        firstError.GetProperty("message").GetString().Should().NotBeNullOrEmpty();
    }

    // ═══════════════════════════════════════════════════════════════
    //  Case 2: Layer 2 error (422 - CsvParser failure)
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task Post_CorruptedCsv_Returns422()
    {
        // Arrange
        var csvPath = Path.Combine(CsvDir, "corrupted.csv");

        // Act
        var response = await PostValidationAsync(csvPath);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        var json = ParseJson(await response.Content.ReadAsStringAsync());
        json.GetProperty("code").GetInt32().Should().Be(422);
        json.GetProperty("msg").GetString().Should().Be("CSV file parsing failed");
    }

    // ═══════════════════════════════════════════════════════════════
    //  Case 3: Layer 3 error (422 - CsvValidator field errors)
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task Post_InvalidFields_Returns422()
    {
        // Arrange
        var csvPath = Path.Combine(CsvDir, "invalid_fields.csv");

        // Act
        var response = await PostValidationAsync(csvPath);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        var json = ParseJson(await response.Content.ReadAsStringAsync());
        json.GetProperty("code").GetInt32().Should().Be(422);
        json.GetProperty("msg").GetString().Should().Be("CSV format validation failed");
    }

    // ═══════════════════════════════════════════════════════════════
    //  Case 4: Success - violations found (status=Failed)
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task Post_UnderpaidEmployee_Returns200Failed()
    {
        // Arrange
        var csvPath = Path.Combine(CsvDir, "underpaid.csv");

        // Act
        var response = await PostValidationAsync(csvPath);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = ParseJson(await response.Content.ReadAsStringAsync());
        json.GetProperty("code").GetInt32().Should().Be(200);

        var data = json.GetProperty("data");
        data.GetProperty("status").GetString().Should().Be("Failed");
        data.GetProperty("issues").GetArrayLength().Should().BeGreaterThan(0);

        // Verify DB state
        var validationId = data.GetProperty("validationId").GetGuid();
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
        var dbIssues = await db.PayrollIssues
            .Where(i => i.PayrollValidationId == validationId)
            .ToListAsync();
        dbIssues.Should().NotBeEmpty();
    }

    // ═══════════════════════════════════════════════════════════════
    //  Case 5: Success - all compliant (status=Passed)
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task Post_CompliantEmployee_Returns200Passed()
    {
        // Arrange
        var csvPath = Path.Combine(CsvDir, "compliant.csv");

        // Act
        var response = await PostValidationAsync(csvPath);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = ParseJson(await response.Content.ReadAsStringAsync());
        json.GetProperty("code").GetInt32().Should().Be(200);

        var data = json.GetProperty("data");
        data.GetProperty("status").GetString().Should().Be("Passed");
        data.GetProperty("issues").GetArrayLength().Should().Be(0);
    }

    // ═══════════════════════════════════════════════════════════════
    //  Case 6: Employee upsert (update, not duplicate)
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task Post_ExistingEmployee_UpdatesNotDuplicates()
    {
        // Arrange: pre-seed an employee with EmployeeNumber "E999"
        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
            var existing = await db.Employees
                .FirstOrDefaultAsync(e => e.EmployeeNumber == "E999" && e.OrganizationId == TestOrganizationId);
            if (existing == null)
            {
                db.Employees.Add(new Employee
                {
                    OrganizationId = TestOrganizationId,
                    FirstName = "Original",
                    LastName = "Employee",
                    JobTitle = "Staff",
                    EmploymentType = EmploymentType.FullTime,
                    StartDate = DateTime.SpecifyKind(new DateTime(2025, 1, 1), DateTimeKind.Utc),
                    AwardType = AwardType.GeneralRetailIndustryAward2020,
                    AwardLevelNumber = 1,
                    EmployeeNumber = "E999",
                });
                await db.SaveChangesAsync();
            }
        }

        var csvPath = Path.Combine(CsvDir, "existing_employee.csv");

        // Act
        var response = await PostValidationAsync(csvPath);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify: no duplicate employees with EmployeeNumber "E999"
        using var verifyScope = Factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();
        var employees = await verifyDb.Employees
            .Where(e => e.EmployeeNumber == "E999" && e.OrganizationId == TestOrganizationId)
            .ToListAsync();

        employees.Should().HaveCount(1);
        employees[0].FirstName.Should().Be("Updated");
        employees[0].LastName.Should().Be("Name");
        employees[0].AwardLevelNumber.Should().Be(2);
    }
}
