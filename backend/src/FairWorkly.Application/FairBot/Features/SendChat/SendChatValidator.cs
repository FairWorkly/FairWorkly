using FluentValidation;

namespace FairWorkly.Application.FairBot.Features.SendChat;

public class SendChatValidator : AbstractValidator<SendChatCommand>
{
    private const int MaxMessageLength = 10_000;
    private const int MaxConversationIdLength = 128;

    public SendChatValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage("Message is required.")
            .MaximumLength(MaxMessageLength)
            .WithMessage(x =>
                $"Message is too long ({x.Message?.Length ?? 0} chars). Maximum is {MaxMessageLength}."
            );

        RuleFor(x => x.RequestId).NotEmpty().WithMessage("RequestId is required.");

        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.OrganizationId).NotEmpty().WithMessage("OrganizationId is required.");

        RuleFor(x => x.ConversationId)
            .MaximumLength(MaxConversationIdLength)
            .WithMessage(
                $"ConversationId is too long. Maximum is {MaxConversationIdLength} characters."
            )
            .When(x => !string.IsNullOrWhiteSpace(x.ConversationId));
    }
}
