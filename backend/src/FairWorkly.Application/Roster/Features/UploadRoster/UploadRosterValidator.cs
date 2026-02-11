using FluentValidation;

namespace FairWorkly.Application.Roster.Features.UploadRoster;

/// <summary>
/// Validator for UploadRosterCommand.
/// Validates file presence, format, size, and required IDs.
/// </summary>
public class UploadRosterValidator : AbstractValidator<UploadRosterCommand>
{
    private static readonly string[] AllowedExtensions = { ".xlsx" };
    private const long MaxFileSizeBytes = 50 * 1024 * 1024; // 50MB

    public UploadRosterValidator()
    {
        RuleFor(x => x.FileStream)
            .NotNull()
            .WithMessage("Roster file is required");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("File name is required")
            .Must(HaveValidExtension)
            .WithMessage($"File must be Excel format ({string.Join(", ", AllowedExtensions)})");

        RuleFor(x => x.FileSize)
            .GreaterThan(0)
            .WithMessage("File must not be empty")
            .LessThanOrEqualTo(MaxFileSizeBytes)
            .WithMessage($"File size must not exceed {MaxFileSizeBytes / (1024 * 1024)}MB");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .WithMessage("Content type is required");

        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("OrganizationId is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required");
    }

    private static bool HaveValidExtension(string? fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return false;
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }
}
