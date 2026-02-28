using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FairWorkly.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace FairWorkly.IntegrationTests.Integration;

public class FairBotChatIntegrationTests : IClassFixture<FairBotTestWebApplicationFactory>
{
    private readonly FairBotTestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public FairBotChatIntegrationTests(FairBotTestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Chat_WithHistoryAndConversationId_ForwardsNormalizedPayloadToAgent()
    {
        _factory.CapturingAiClient.Reset();

        var token = await GetAccessTokenAsync();
        var client = CreateAuthenticatedClient(token);

        var historyPayloadJson = JsonSerializer.Serialize(
            new[]
            {
                new { role = "assistant", content = "Previous assistant answer." },
                new { role = "user", content = "What should we do next?" },
            }
        );

        using var form = new MultipartFormDataContent
        {
            { new StringContent("Give me next steps."), "message" },
            { new StringContent("compliance"), "intentHint" },
            { new StringContent(historyPayloadJson), "historyPayload" },
            { new StringContent("conv-history-001"), "conversationId" },
        };

        var response = await client.PostAsync("/api/fairbot/chat", form);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("code").GetInt32().Should().Be(200);
        doc.RootElement.GetProperty("data")
            .GetProperty("status")
            .GetString()
            .Should()
            .Be("success");

        _factory.CapturingAiClient.PostFormCallCount.Should().Be(1);
        _factory.CapturingAiClient.LastRoute.Should().Be("/api/agent/chat");
        _factory.CapturingAiClient.LastFormFields.Should().NotBeNull();

        var fields = _factory.CapturingAiClient.LastFormFields!;
        fields.Should().ContainKey("message");
        fields["message"].Should().Be("Give me next steps.");
        fields.Should().ContainKey("intent_hint");
        fields["intent_hint"].Should().Be("compliance");
        fields.Should().ContainKey("conversation_id");
        fields["conversation_id"].Should().Be("conv-history-001");
        fields.Should().ContainKey("history_payload");

        using var historyDoc = JsonDocument.Parse(fields["history_payload"]);
        historyDoc.RootElement.ValueKind.Should().Be(JsonValueKind.Array);
        historyDoc.RootElement.GetArrayLength().Should().Be(2);
        historyDoc.RootElement[0].GetProperty("role").GetString().Should().Be("assistant");
        historyDoc.RootElement[1].GetProperty("role").GetString().Should().Be("user");
    }

    [Fact]
    public async Task Chat_WithInvalidHistoryRole_Returns422AndDoesNotCallAgent()
    {
        _factory.CapturingAiClient.Reset();

        var token = await GetAccessTokenAsync();
        var client = CreateAuthenticatedClient(token);

        var invalidHistoryJson = JsonSerializer.Serialize(
            new[] { new { role = "system", content = "Forbidden role" } }
        );

        using var form = new MultipartFormDataContent
        {
            { new StringContent("Test invalid history."), "message" },
            { new StringContent(invalidHistoryJson), "historyPayload" },
        };

        var response = await client.PostAsync("/api/fairbot/chat", form);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("code").GetInt32().Should().Be(422);
        doc.RootElement.GetProperty("msg")
            .GetString()
            .Should()
            .Be("History item role must be either 'user' or 'assistant'.");

        _factory.CapturingAiClient.PostFormCallCount.Should().Be(0);
    }

    [Fact]
    public async Task Chat_ResponseIncludesStructuredResultData_FromAgent()
    {
        _factory.CapturingAiClient.Reset();

        var token = await GetAccessTokenAsync();
        var client = CreateAuthenticatedClient(token);

        using var form = new MultipartFormDataContent
        {
            { new StringContent("Explain this result."), "message" },
        };

        var response = await client.PostAsync("/api/fairbot/chat", form);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);

        var result = doc.RootElement.GetProperty("data").GetProperty("result");
        var actionPlan = result.GetProperty("data").GetProperty("action_plan");

        actionPlan.GetProperty("title").GetString().Should().Be("Top 3 actions to fix this roster");
        actionPlan.GetProperty("actions").GetArrayLength().Should().BeGreaterThan(0);
        actionPlan.GetProperty("quick_follow_ups").GetArrayLength().Should().BeGreaterThan(0);
    }

    private async Task<string> GetAccessTokenAsync(
        string email = "test@example.com",
        string password = "TestPassword123"
    )
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new { email, password });
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("data").GetProperty("accessToken").GetString()!;
    }

    private HttpClient CreateAuthenticatedClient(string token)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
