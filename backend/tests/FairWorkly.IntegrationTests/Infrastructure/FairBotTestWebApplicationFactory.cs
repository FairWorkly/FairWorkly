using System.Text.Json;
using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.FairBot;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FairWorkly.IntegrationTests.Infrastructure;

public class FairBotTestWebApplicationFactory : CustomWebApplicationFactory
{
    public CapturingAiClient CapturingAiClient { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IAiClient>();
            services.AddSingleton(CapturingAiClient);
            services.AddSingleton<IAiClient>(sp => sp.GetRequiredService<CapturingAiClient>());
        });
    }
}

public sealed class CapturingAiClient : IAiClient
{
    private readonly object _lock = new();

    public int PostFormCallCount { get; private set; }
    public string? LastRoute { get; private set; }
    public Dictionary<string, string>? LastFormFields { get; private set; }
    public Dictionary<string, string>? LastHeaders { get; private set; }

    public void Reset()
    {
        lock (_lock)
        {
            PostFormCallCount = 0;
            LastRoute = null;
            LastFormFields = null;
            LastHeaders = null;
        }
    }

    public Task<TResponse> PostAsync<TRequest, TResponse>(
        string route,
        TRequest request,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotSupportedException("PostAsync is not used in FairBot tests.");
    }

    public Task<TResponse> PostMultipartAsync<TResponse>(
        string route,
        Stream fileStream,
        string fileName,
        string contentType,
        string message,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotSupportedException("PostMultipartAsync is not used in FairBot tests.");
    }

    public Task<TResponse> PostFormAsync<TResponse>(
        string route,
        Dictionary<string, string> formFields,
        Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default
    )
    {
        lock (_lock)
        {
            PostFormCallCount++;
            LastRoute = route;
            LastFormFields = new Dictionary<string, string>(formFields);
            LastHeaders = headers == null ? null : new Dictionary<string, string>(headers);
        }

        if (typeof(TResponse) != typeof(FairBotChatResponse))
        {
            throw new InvalidOperationException(
                $"Unexpected response type requested: {typeof(TResponse).Name}"
            );
        }

        var response = new FairBotChatResponse
        {
            Status = "success",
            Message = "ok",
            RequestId =
                headers != null && headers.TryGetValue("X-Request-Id", out var id)
                    ? id
                    : "test-request-id",
            RoutedTo = "compliance_qa",
            Result = new FairBotChatResult
            {
                Type = "compliance",
                Message = "Stubbed integration response",
                Model = "test-model",
                Data = JsonDocument
                    .Parse(
                        """
                        {
                          "action_plan": {
                            "title": "Top 3 actions to fix this roster",
                            "validation_id": "val-test-001",
                            "actions": [
                              {
                                "id": "action_1",
                                "priority": "P1",
                                "title": "Fix meal break scheduling",
                                "owner": "Roster manager",
                                "check_type": "MealBreak",
                                "issue_count": 3,
                                "critical_count": 2,
                                "affected_shifts": [],
                                "what_to_change": "Adjust break timing to meet minimum rules.",
                                "why": "Break breaches are direct non-compliance.",
                                "expected_outcome": "Meal-break errors are removed.",
                                "risk_if_ignored": "Repeated compliance failures.",
                                "focus_examples": "Sarah Chen (2026-02-10)"
                              }
                            ],
                            "quick_follow_ups": [
                              {
                                "id": "follow_up_action_1",
                                "label": "Expand P1",
                                "prompt": "Give me steps for meal breaks.",
                                "action_id": "action_1"
                              }
                            ]
                          }
                        }
                        """
                    )
                    .RootElement.Clone(),
            },
        };

        return Task.FromResult((TResponse)(object)response);
    }
}
