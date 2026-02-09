using FairWorkly.Domain.Common.Enums;

namespace FairWorkly.Domain.Roster.Parameters;

public interface IRosterRuleParametersProvider
{
    RosterRuleParameterSet Get(AwardType awardType);
}

