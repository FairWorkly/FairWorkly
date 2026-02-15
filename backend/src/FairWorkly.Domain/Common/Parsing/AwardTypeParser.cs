using FairWorkly.Domain.Common.Enums;

namespace FairWorkly.Domain.Common.Parsing;

public static class AwardTypeParser
{
    public static bool TryParse(string? value, out AwardType awardType)
    {
        awardType = default;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        var normalized = value.Trim().ToLowerInvariant();

        awardType = normalized switch
        {
            "retail" => AwardType.GeneralRetailIndustryAward2020,
            "hospitality" => AwardType.HospitalityIndustryAward2020,
            "clerks" => AwardType.ClerksPrivateSectorAward2020,
            _ => default,
        };

        return awardType != default;
    }

    public static string ToShortName(AwardType awardType)
    {
        return awardType switch
        {
            AwardType.GeneralRetailIndustryAward2020 => "Retail",
            AwardType.HospitalityIndustryAward2020 => "Hospitality",
            AwardType.ClerksPrivateSectorAward2020 => "Clerks",
            _ => awardType.ToString(),
        };
    }
}
