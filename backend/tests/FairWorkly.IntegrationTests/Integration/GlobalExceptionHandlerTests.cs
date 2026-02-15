using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using FairWorkly.Application.Common.Interfaces;
using FairWorkly.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FairWorkly.IntegrationTests.Integration;

public class GlobalExceptionHandlerTests : IntegrationTestBase
{
    private static readonly string CsvDir = Path.Combine("TestData", "Csv", "Payroll");

    public GlobalExceptionHandlerTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Post_SaveChangesThrows_Returns500ProblemDetails()
    {
        // Get token from normal factory (login also calls SaveChanges for refresh token)
        var token = await GetAccessTokenAsync();

        // Create isolated client with IUnitOfWork that throws
        var client = Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IUnitOfWork>();
                services.AddScoped<IUnitOfWork, ThrowingUnitOfWork>();
            });
        }).CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Send valid request that passes Layer 1-3
        var csvPath = Path.Combine(CsvDir, "compliant.csv");
        using var content = new MultipartFormDataContent();
        await using var fileStream = File.OpenRead(csvPath);
        content.Add(new StreamContent(fileStream), "file", "compliant.csv");
        content.Add(new StringContent("GeneralRetailIndustryAward2020"), "awardType");
        content.Add(new StringContent("VIC"), "state");

        // Act
        var response = await client.PostAsync("/api/payroll/validation", content);

        // Assert: 500 ProblemDetails
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        json.GetProperty("status").GetInt32().Should().Be(500);
        json.GetProperty("title").GetString().Should().Be("Internal Server Error");
        json.GetProperty("detail").GetString().Should().Be("An error occurred while processing your request.");
    }

    private class ThrowingUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => throw new DbUpdateException("Simulated database failure");
    }
}
