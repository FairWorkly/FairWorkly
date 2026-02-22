using FairWorkly.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace FairWorkly.IntegrationTests.Infrastructure;

public class PostgresConnectionTests : IntegrationTestBase
{
    public PostgresConnectionTests(CustomWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task CanConnectToPostgres()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();

        var canConnect = await db.Database.CanConnectAsync();

        canConnect.Should().BeTrue();
    }

    [Fact]
    public async Task CanGetAccessToken()
    {
        var token = await GetAccessTokenAsync();

        token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CanMakeAuthenticatedRequest()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/auth/me");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}
