using FairWorkly.Domain.Common.Enums;

namespace FairWorkly.Domain.Common.Parsing;

public static class EmploymentTypeParser
{
    public static bool TryParse(string? value, out EmploymentType employmentType)
    {
        employmentType = default;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        var normalized = value.Trim().ToLowerInvariant();
        normalized = normalized.Replace("-", "").Replace("_", "").Replace(" ", "");

        employmentType = normalized switch
        {
            "fulltime" or "ft" => EmploymentType.FullTime,
            "parttime" or "pt" => EmploymentType.PartTime,
            "casual" or "cas" => EmploymentType.Casual,
            "fixedterm" => EmploymentType.FixedTerm,
            _ => default,
        };

        return employmentType != default;
    }

    public static EmploymentType ParseOrDefault(
        string? value,
        EmploymentType defaultValue = EmploymentType.FullTime
    )
    {
        return TryParse(value, out var parsed) ? parsed : defaultValue;
    }
}
