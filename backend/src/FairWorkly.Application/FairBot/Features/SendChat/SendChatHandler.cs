using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Roster.Features.GetValidationResults;
using FairWorkly.Domain.Common.Result;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FairWorkly.Application.FairBot.Features.SendChat;

public class SendChatHandler(
    IAiClient aiClient,
    IMediator mediator,
    IConfiguration configuration,
    ILogger<SendChatHandler> logger
) : IRequestHandler<SendChatCommand, Result<FairBotChatResponse>>
{
    private const int MaxContextPayloadBytes = 512_000; // 500 KB
    private const int MaxHistoryPayloadBytes = 128_000; // 125 KB
    private const int MaxHistoryMessages = 20;
    private const int MaxHistoryMessageChars = 2_000;
    private const int MaxConversationIdLength = 128;

    private static readonly Regex ValidConversationIdPattern = new(
        @"^[\w\-:.]+$",
        RegexOptions.Compiled
    );

    private static readonly JsonSerializerOptions ContextPayloadSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() },
    };

    private static readonly JsonSerializerOptions HistoryPayloadSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly int _agentTimeoutSeconds = Math.Max(
        configuration.GetValue<int?>("AiSettings:TimeoutSeconds") ?? 120,
        1
    );

    public async Task<Result<FairBotChatResponse>> Handle(
        SendChatCommand request,
        CancellationToken cancellationToken
    )
    {
        var stopwatch = Stopwatch.StartNew();

        using var logScope = logger.BeginScope(
            new Dictionary<string, object?>
            {
                ["RequestId"] = request.RequestId,
                ["OrgId"] = request.OrganizationId,
                ["UserId"] = request.UserId,
            }
        );

        logger.LogInformation(
            "FairBot chat received: intentHint={IntentHint}, messageLength={MessageLength}",
            request.IntentHint,
            request.Message.Length
        );

        var formFields = new Dictionary<string, string> { { "message", request.Message } };

        if (!string.IsNullOrWhiteSpace(request.IntentHint))
        {
            formFields["intent_hint"] = request.IntentHint;
        }

        if (!string.IsNullOrWhiteSpace(request.ConversationId))
        {
            var normalizedConversationId = request.ConversationId.Trim();
            if (
                normalizedConversationId.Length > MaxConversationIdLength
                || !ValidConversationIdPattern.IsMatch(normalizedConversationId)
            )
            {
                return Result<FairBotChatResponse>.Of422(
                    "ConversationId format is invalid. Please retry from a fresh FairBot session."
                );
            }
            formFields["conversation_id"] = normalizedConversationId;
        }

        // Roster intent: enrich context with full validation data from DB
        if (string.Equals(request.IntentHint, "roster", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogInformation(
                "FairBot roster intent: contextPayload={HasPayload}",
                !string.IsNullOrWhiteSpace(request.ContextPayload)
            );

            var contextResolution = await ResolveRosterContextPayloadAsync(
                request.ContextPayload,
                request.OrganizationId,
                cancellationToken
            );

            if (contextResolution is { IsSuccess: true })
            {
                if (
                    ExceedsPayloadLimit(
                        contextResolution.ContextPayload!,
                        MaxContextPayloadBytes,
                        out var rosterBytes
                    )
                )
                {
                    return Result<FairBotChatResponse>.Of413(
                        $"Context payload is too large ({rosterBytes} bytes). Maximum is {MaxContextPayloadBytes}."
                    );
                }
                logger.LogInformation("FairBot roster context resolved successfully");
                formFields["context_payload"] = contextResolution.ContextPayload!;
            }
            else
            {
                logger.LogWarning(
                    "FairBot roster context resolution failed: {Reason}. Falling back to compliance Q&A",
                    contextResolution?.ErrorMessage ?? "resolution returned null"
                );
                // Drop roster intent — let agent route to compliance Q&A.
                formFields.Remove("intent_hint");
            }
        }
        else if (!string.IsNullOrWhiteSpace(request.ContextPayload))
        {
            if (
                ExceedsPayloadLimit(
                    request.ContextPayload,
                    MaxContextPayloadBytes,
                    out var payloadBytes
                )
            )
            {
                return Result<FairBotChatResponse>.Of413(
                    $"Context payload is too large ({payloadBytes} bytes). Maximum is {MaxContextPayloadBytes}."
                );
            }
            formFields["context_payload"] = request.ContextPayload;
        }

        if (!string.IsNullOrWhiteSpace(request.HistoryPayload))
        {
            if (
                ExceedsPayloadLimit(
                    request.HistoryPayload,
                    MaxHistoryPayloadBytes,
                    out var historyBytes
                )
            )
            {
                return Result<FairBotChatResponse>.Of413(
                    $"History payload is too large ({historyBytes} bytes). Maximum is {MaxHistoryPayloadBytes}."
                );
            }

            if (
                !TryNormalizeHistoryPayload(
                    request.HistoryPayload,
                    out var normalizedHistoryPayload,
                    out var historyErrorMessage
                )
            )
            {
                return Result<FairBotChatResponse>.Of422(historyErrorMessage!);
            }

            formFields["history_payload"] = normalizedHistoryPayload!;
        }

        logger.LogInformation(
            "FairBot sending to agent: fields=[{Fields}], contextPayloadLength={ContextLength}, historyPayloadLength={HistoryLength}, hasConversationId={HasConversationId}",
            string.Join(", ", formFields.Keys),
            formFields.TryGetValue("context_payload", out var cp) ? cp.Length : -1,
            formFields.TryGetValue("history_payload", out var hp) ? hp.Length : -1,
            formFields.ContainsKey("conversation_id")
        );

        try
        {
            var agentResponse = await aiClient.PostFormAsync<FairBotChatResponse>(
                "/api/agent/chat",
                formFields,
                new Dictionary<string, string> { ["X-Request-Id"] = request.RequestId },
                cancellationToken
            );
            stopwatch.Stop();
            logger.LogInformation(
                "FairBot chat completed: routedTo={RoutedTo}, note={Note}, upstreamRequestId={UpstreamRequestId}, elapsedMs={ElapsedMs}",
                agentResponse.RoutedTo,
                agentResponse.Result?.Note,
                agentResponse.RequestId,
                stopwatch.ElapsedMilliseconds
            );

            return Result<FairBotChatResponse>.Of200("Chat response received", agentResponse);
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            stopwatch.Stop();
            logger.LogWarning(
                ex,
                "FairBot chat timed out while waiting for agent: elapsedMs={ElapsedMs}",
                stopwatch.ElapsedMilliseconds
            );
            return Result<FairBotChatResponse>.Of504(
                $"This analysis timed out ({_agentTimeoutSeconds}s), so we couldn't get a complete result. "
                    + "Please try again. If it still fails, narrow your question or contact support."
            );
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();
            logger.LogWarning(
                ex,
                "FairBot chat failed to reach agent: elapsedMs={ElapsedMs}",
                stopwatch.ElapsedMilliseconds
            );
            return MapAgentServiceError(ex);
        }
    }

    // ═══════════════════════════════════════════════════════════════════
    // Agent Service error mapping
    // ═══════════════════════════════════════════════════════════════════

    private Result<FairBotChatResponse> MapAgentServiceError(HttpRequestException ex)
    {
        return ex.StatusCode switch
        {
            HttpStatusCode.TooManyRequests => Result<FairBotChatResponse>.Of429(
                "FairBot is handling too many requests right now. Please wait a moment and try again."
            ),
            HttpStatusCode.RequestEntityTooLarge => Result<FairBotChatResponse>.Of413(
                "Your request is too large. Please shorten your message and try again."
            ),
            HttpStatusCode.Unauthorized => Result<FairBotChatResponse>.Of401("Unauthorized"),
            HttpStatusCode.GatewayTimeout or HttpStatusCode.RequestTimeout =>
                Result<FairBotChatResponse>.Of504(
                    $"This analysis timed out ({_agentTimeoutSeconds}s), so we couldn't get a complete result. "
                        + "Please try again. If it still fails, narrow your question or contact support."
                ),
            _ => Result<FairBotChatResponse>.Of500(
                "Unable to reach the AI service. Please try again later."
            ),
        };
    }

    // ═══════════════════════════════════════════════════════════════════
    // Roster context enrichment
    // ═══════════════════════════════════════════════════════════════════

    private async Task<ResolvedRosterContext> ResolveRosterContextPayloadAsync(
        string? rawContextPayload,
        Guid organizationId,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(rawContextPayload))
        {
            return ResolvedRosterContext.Fail(
                "Roster context is required. Please open FairBot from a roster results page."
            );
        }

        if (!TryParseContextReference(rawContextPayload, out var reference))
        {
            return ResolvedRosterContext.Fail(
                "Roster context format is invalid. Please open FairBot from a roster results page."
            );
        }

        if (reference?.RosterId is not { } rosterId)
        {
            return ResolvedRosterContext.Fail(
                "Roster context is missing rosterId. Please open FairBot from a roster results page."
            );
        }

        var queryResult = await mediator.Send(
            new GetValidationResultsQuery { RosterId = rosterId, OrganizationId = organizationId },
            cancellationToken
        );

        if (!queryResult.IsSuccess || queryResult.Value == null)
        {
            return ResolvedRosterContext.Fail(
                "Roster validation context could not be loaded. Please re-open FairBot from the latest roster results page."
            );
        }

        if (
            reference.ValidationId is { } expectedValidationId
            && queryResult.Value.ValidationId != expectedValidationId
        )
        {
            return ResolvedRosterContext.Fail(
                "Roster validation context is out of date. Please re-open FairBot from the latest roster results page."
            );
        }

        var normalizedPayload = JsonSerializer.Serialize(
            new Dictionary<string, object?>
            {
                ["kind"] = "roster_validation",
                ["rosterId"] = rosterId,
                ["validation"] = queryResult.Value,
            },
            ContextPayloadSerializerOptions
        );

        return ResolvedRosterContext.Ok(normalizedPayload);
    }

    // ═══════════════════════════════════════════════════════════════════
    // JSON parsing helpers
    // ═══════════════════════════════════════════════════════════════════

    private static bool TryParseContextReference(
        string rawContextPayload,
        out RosterContextReference? reference
    )
    {
        reference = null;

        try
        {
            using var doc = JsonDocument.Parse(rawContextPayload);
            var root = doc.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
            {
                return false;
            }

            if (!TryReadGuid(root, "rosterId", out var rosterId))
            {
                return false;
            }

            Guid? validationId = null;
            if (TryReadGuid(root, "validationId", out var parsedValidationId))
            {
                validationId = parsedValidationId;
            }
            reference = new RosterContextReference(rosterId, validationId);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool TryReadGuid(JsonElement root, string propertyName, out Guid value)
    {
        value = Guid.Empty;
        if (!root.TryGetProperty(propertyName, out var element))
        {
            return false;
        }

        if (element.ValueKind != JsonValueKind.String)
        {
            return false;
        }

        var raw = element.GetString();
        return Guid.TryParse(raw, out value);
    }

    private static bool ExceedsPayloadLimit(string payload, int maxBytes, out int byteCount)
    {
        byteCount = Encoding.UTF8.GetByteCount(payload);
        return byteCount > maxBytes;
    }

    private static bool TryNormalizeHistoryPayload(
        string rawHistoryPayload,
        out string? normalizedPayload,
        out string? errorMessage
    )
    {
        normalizedPayload = null;
        errorMessage = null;

        try
        {
            using var doc = JsonDocument.Parse(rawHistoryPayload);
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
            {
                errorMessage = "History payload must be a JSON array.";
                return false;
            }

            var normalized = new List<HistoryMessage>();
            foreach (var item in doc.RootElement.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object)
                {
                    errorMessage = "History payload items must be JSON objects.";
                    return false;
                }

                if (
                    !item.TryGetProperty("role", out var roleElement)
                    || roleElement.ValueKind != JsonValueKind.String
                )
                {
                    errorMessage = "History item role is required.";
                    return false;
                }

                if (
                    !item.TryGetProperty("content", out var contentElement)
                    || contentElement.ValueKind != JsonValueKind.String
                )
                {
                    errorMessage = "History item content is required.";
                    return false;
                }

                var role = roleElement.GetString()?.Trim().ToLowerInvariant();
                var content = contentElement.GetString()?.Trim() ?? string.Empty;

                if (role is not ("user" or "assistant"))
                {
                    errorMessage = "History item role must be either 'user' or 'assistant'.";
                    return false;
                }

                if (content.Length == 0)
                {
                    continue;
                }

                if (content.Length > MaxHistoryMessageChars)
                {
                    content = content[..MaxHistoryMessageChars];
                }

                normalized.Add(new HistoryMessage(role, content));
            }

            var bounded = normalized.TakeLast(MaxHistoryMessages).ToList();
            normalizedPayload = JsonSerializer.Serialize(bounded, HistoryPayloadSerializerOptions);
            return true;
        }
        catch (JsonException)
        {
            errorMessage = "History payload format is invalid JSON.";
            return false;
        }
    }

    // ═══════════════════════════════════════════════════════════════════
    // Helper types
    // ═══════════════════════════════════════════════════════════════════

    private sealed record RosterContextReference(Guid? RosterId, Guid? ValidationId);

    private sealed record HistoryMessage(string Role, string Content);

    private sealed record ResolvedRosterContext(
        bool IsSuccess,
        string? ContextPayload,
        string? ErrorMessage
    )
    {
        public static ResolvedRosterContext Ok(string contextPayload) =>
            new(true, contextPayload, null);

        public static ResolvedRosterContext Fail(string errorMessage) =>
            new(false, null, errorMessage);
    }
}
