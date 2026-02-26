using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.FairBot;
using FairWorkly.Application.Roster.Features.GetValidationResults;
using FairWorkly.Domain.Common.Result;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FairWorkly.API.Controllers.FairBot;

/// <summary>
/// Proxies FairBot chat requests to the Agent Service.
/// Ensures all AI chat calls are authenticated and authorized via JWT.
/// </summary>
[Route("api/fairbot")]
[Authorize(Policy = "RequireAdmin")]
public class FairBotController(
    IAiClient aiClient,
    ICurrentUserService currentUser,
    IMediator mediator,
    IConfiguration configuration,
    ILogger<FairBotController> logger
) : BaseApiController
{
    private const int MaxMessageLength = 10_000;
    private const int MaxRequestIdLength = 128;
    private static readonly Regex ValidRequestIdPattern = new(
        @"^[\w\-:.]+$",
        RegexOptions.Compiled
    );

    private static readonly JsonSerializerOptions ContextPayloadSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly IAiClient _aiClient = aiClient;
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly IMediator _mediator = mediator;
    private readonly int _agentTimeoutSeconds = Math.Max(
        configuration.GetValue<int?>("AiSettings:TimeoutSeconds") ?? 120,
        1
    );
    private readonly ILogger<FairBotController> _logger = logger;

    /// <summary>
    /// Send a chat message to FairBot via the Agent Service.
    /// Supports compliance Q&amp;A, roster explanation, and payroll verification.
    /// </summary>
    [HttpPost("chat")]
    public async Task<IActionResult> Chat(
        [FromForm] string message,
        [FromForm] string? intentHint = null,
        [FromForm(Name = "intent_hint")] string? intentHintLegacy = null,
        [FromForm] string? contextPayload = null,
        [FromForm(Name = "context_payload")] string? contextPayloadLegacy = null,
        [FromHeader(Name = "X-Request-Id")] string? requestIdHeader = null,
        CancellationToken cancellationToken = default
    )
    {
        var requestId = SanitizeRequestId(requestIdHeader) ?? HttpContext.TraceIdentifier;
        Response.Headers["X-Request-Id"] = requestId;
        var stopwatch = Stopwatch.StartNew();

        using var logScope = _logger.BeginScope(
            new Dictionary<string, object?>
            {
                ["RequestId"] = requestId,
                ["OrgId"] = _currentUser.OrganizationId,
                ["UserId"] = _currentUser.UserId,
            }
        );

        _logger.LogInformation(
            "FairBot chat received: intentHint={IntentHint}, messageLength={MessageLength}",
            intentHint ?? intentHintLegacy,
            message?.Length ?? 0
        );

        if (string.IsNullOrWhiteSpace(message))
        {
            return StatusCode(400, new { code = 400, msg = "Message is required." });
        }

        if (message.Length > MaxMessageLength)
        {
            return StatusCode(
                400,
                new
                {
                    code = 400,
                    msg = $"Message is too long ({message.Length} chars). Maximum is {MaxMessageLength}.",
                }
            );
        }

        if (_currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return RespondResult(Result<FairBotChatResponse>.Of401("Invalid user token"));
        }

        var resolvedIntentHint = !string.IsNullOrWhiteSpace(intentHint)
            ? intentHint
            : intentHintLegacy;

        var resolvedContextPayload = !string.IsNullOrWhiteSpace(contextPayload)
            ? contextPayload
            : contextPayloadLegacy;

        var formFields = new Dictionary<string, string> { { "message", message } };

        if (!string.IsNullOrWhiteSpace(resolvedIntentHint))
        {
            formFields["intent_hint"] = resolvedIntentHint;
        }

        if (string.Equals(resolvedIntentHint, "roster", StringComparison.OrdinalIgnoreCase))
        {
            // Try to enrich with full roster validation data.
            // If it fails (missing context, stale data, etc.), gracefully fall back
            // to general compliance Q&A instead of returning an error.
            _logger.LogInformation(
                "FairBot roster intent: contextPayload={HasPayload}",
                !string.IsNullOrWhiteSpace(resolvedContextPayload)
            );

            ResolvedRosterContext? contextResolution = null;
            if (_currentUser.OrganizationId is { } organizationId && organizationId != Guid.Empty)
            {
                contextResolution = await ResolveRosterContextPayloadAsync(
                    resolvedContextPayload,
                    organizationId,
                    cancellationToken
                );
            }

            if (contextResolution is { IsSuccess: true })
            {
                _logger.LogInformation("FairBot roster context resolved successfully");
                formFields["context_payload"] = contextResolution.ContextPayload!;
            }
            else
            {
                _logger.LogWarning(
                    "FairBot roster context resolution failed: {Reason}. Falling back to compliance Q&A",
                    contextResolution?.ErrorMessage ?? "OrganizationId missing or empty"
                );
                // Drop roster intent — let agent route to compliance Q&A.
                formFields.Remove("intent_hint");
            }
        }
        else if (!string.IsNullOrWhiteSpace(resolvedContextPayload))
        {
            formFields["context_payload"] = resolvedContextPayload;
        }

        _logger.LogInformation(
            "FairBot sending to agent: fields=[{Fields}], contextPayloadLength={Length}",
            string.Join(", ", formFields.Keys),
            formFields.TryGetValue("context_payload", out var cp) ? cp.Length : -1
        );

        try
        {
            var agentResponse = await _aiClient.PostFormAsync<FairBotChatResponse>(
                "/api/agent/chat",
                formFields,
                new Dictionary<string, string> { ["X-Request-Id"] = requestId },
                cancellationToken
            );
            stopwatch.Stop();
            _logger.LogInformation(
                "FairBot chat completed: routedTo={RoutedTo}, note={Note}, upstreamRequestId={UpstreamRequestId}, elapsedMs={ElapsedMs}",
                agentResponse.RoutedTo,
                agentResponse.Result?.Note,
                agentResponse.RequestId,
                stopwatch.ElapsedMilliseconds
            );

            return RespondResult(
                Result<FairBotChatResponse>.Of200("Chat response received", agentResponse)
            );
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            stopwatch.Stop();
            _logger.LogWarning(
                ex,
                "FairBot chat timed out while waiting for agent: elapsedMs={ElapsedMs}",
                stopwatch.ElapsedMilliseconds
            );
            return StatusCode(
                504,
                new
                {
                    code = 504,
                    msg = $"This analysis timed out ({_agentTimeoutSeconds}s), so we couldn't get a complete result. Please try again. If it still fails, narrow your question or contact support.",
                }
            );
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();
            _logger.LogWarning(
                ex,
                "FairBot chat failed to reach agent: elapsedMs={ElapsedMs}",
                stopwatch.ElapsedMilliseconds
            );
            if (ex.StatusCode == HttpStatusCode.TooManyRequests)
            {
                return StatusCode(
                    429,
                    new
                    {
                        code = 429,
                        msg = "FairBot is handling too many requests right now. Please wait a moment and try again.",
                    }
                );
            }

            if (ex.StatusCode == HttpStatusCode.RequestEntityTooLarge)
            {
                return StatusCode(
                    413,
                    new
                    {
                        code = 413,
                        msg = "Your request is too large. Please shorten your message and try again.",
                    }
                );
            }

            if (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                return StatusCode(401, new { code = 401, msg = "Unauthorized" });
            }

            if (ex.StatusCode is HttpStatusCode.GatewayTimeout or HttpStatusCode.RequestTimeout)
            {
                return StatusCode(
                    504,
                    new
                    {
                        code = 504,
                        msg = $"This analysis timed out ({_agentTimeoutSeconds}s), so we couldn't get a complete result. Please try again. If it still fails, narrow your question or contact support.",
                    }
                );
            }

            return StatusCode(
                500,
                new { code = 500, msg = "Unable to reach the AI service. Please try again later." }
            );
        }
    }

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

        var queryResult = await _mediator.Send(
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

    private static string? SanitizeRequestId(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        var trimmed = raw.Trim();
        if (trimmed.Length > MaxRequestIdLength)
        {
            return null;
        }

        return ValidRequestIdPattern.IsMatch(trimmed) ? trimmed : null;
    }

    private sealed record RosterContextReference(Guid? RosterId, Guid? ValidationId);

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
