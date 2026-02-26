using System.Text.RegularExpressions;
using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.FairBot;
using FairWorkly.Application.FairBot.Features.SendChat;
using FairWorkly.Domain.Common.Result;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FairWorkly.API.Controllers.FairBot;

/// <summary>
/// Thin controller for FairBot chat requests.
/// All business logic lives in <see cref="SendChatHandler"/>.
/// </summary>
[Route("api/fairbot")]
[Authorize(Policy = "RequireAdmin")]
public class FairBotController(IMediator mediator, ICurrentUserService currentUser)
    : BaseApiController
{
    private const int MaxRequestIdLength = 128;
    private static readonly Regex ValidRequestIdPattern = new(
        @"^[\w\-:.]+$",
        RegexOptions.Compiled
    );

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
        // HTTP concern: sanitize and echo request ID
        var requestId = SanitizeRequestId(requestIdHeader) ?? HttpContext.TraceIdentifier;
        Response.Headers["X-Request-Id"] = requestId;

        // Auth guard (same pattern as RosterController)
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
            return RespondResult(Result<FairBotChatResponse>.Of401("Invalid user token"));

        if (currentUser.OrganizationId is not { } organizationId || organizationId == Guid.Empty)
            return RespondResult(Result<FairBotChatResponse>.Of401("Invalid user token"));

        // Resolve legacy snake_case form params
        var command = new SendChatCommand
        {
            Message = message ?? string.Empty,
            IntentHint = !string.IsNullOrWhiteSpace(intentHint) ? intentHint : intentHintLegacy,
            ContextPayload = !string.IsNullOrWhiteSpace(contextPayload)
                ? contextPayload
                : contextPayloadLegacy,
            RequestId = requestId,
            UserId = userId,
            OrganizationId = organizationId,
        };

        var result = await mediator.Send(command, cancellationToken);
        return RespondResult(result);
    }

    private static string? SanitizeRequestId(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        var trimmed = raw.Trim();
        if (trimmed.Length > MaxRequestIdLength)
            return null;

        return ValidRequestIdPattern.IsMatch(trimmed) ? trimmed : null;
    }
}
