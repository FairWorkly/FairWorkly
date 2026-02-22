using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Roster.Entities;
using FairWorkly.Domain.Roster.Enums;

namespace FairWorkly.Application.Roster.Features.ValidateRoster;

public enum ValidationFailureType
{
    Compliance,
    Execution,
}

public static class ValidationFailureMarker
{
    // TODO(next-iteration): Replace note-prefix based failure typing with explicit
    // ValidationStatus values (e.g. FailedCompliance / FailedExecution) and
    // migrate existing records. This marker is a low-risk transitional approach.
    private const string ExecutionFailurePrefix = "ExecutionFailure:";

    public static string BuildExecutionFailureNote(string message)
    {
        return $"{ExecutionFailurePrefix} {message}".Trim();
    }

    public static bool IsExecutionFailure(RosterValidation validation)
    {
        return validation.Status == ValidationStatus.Failed
            && !string.IsNullOrWhiteSpace(validation.Notes)
            && validation.Notes.StartsWith(ExecutionFailurePrefix, StringComparison.Ordinal);
    }

    public static ValidationFailureType? GetFailureType(RosterValidation validation)
    {
        if (validation.Status != ValidationStatus.Failed)
        {
            return null;
        }

        return IsExecutionFailure(validation)
            ? ValidationFailureType.Execution
            : ValidationFailureType.Compliance;
    }

    public static bool? IsRetriable(RosterValidation validation)
    {
        return GetFailureType(validation) switch
        {
            ValidationFailureType.Execution => true,
            ValidationFailureType.Compliance => false,
            _ => null,
        };
    }
}
