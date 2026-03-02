using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.FairBot.Features.SendChat;

public class SendChatCommand : IRequest<Result<FairBotChatResponse>>
{
    /// <summary>User's chat message text.</summary>
    public string Message { get; set; } = null!;

    /// <summary>
    /// Resolved intent hint (camelCase/snake_case already resolved by controller).
    /// Null means let the agent decide.
    /// </summary>
    public string? IntentHint { get; set; }

    /// <summary>
    /// Raw context payload JSON string from the frontend.
    /// For roster intent, contains { rosterId, validationId }.
    /// For other intents, passed through as-is.
    /// </summary>
    public string? ContextPayload { get; set; }

    /// <summary>
    /// Raw chat history JSON from frontend.
    /// Format: [{ role: "user"|"assistant", content: "..." }, ...]
    /// </summary>
    public string? HistoryPayload { get; set; }

    /// <summary>
    /// Client-generated conversation ID (for forward compatibility with future persisted threads).
    /// </summary>
    public string? ConversationId { get; set; }

    /// <summary>
    /// Request ID for correlation (sanitized by controller from X-Request-Id header
    /// or HttpContext.TraceIdentifier).
    /// </summary>
    public string RequestId { get; set; } = null!;

    /// <summary>Authenticated user ID from JWT.</summary>
    public Guid UserId { get; set; }

    /// <summary>Authenticated organization ID from JWT.</summary>
    public Guid OrganizationId { get; set; }
}
