namespace FairWorkly.Application.Payroll.Services.ComplianceEngine;

/// <summary>
/// Static provider for Retail Award (MA000004) rate tables
/// Effective from: 01/07/2025
/// </summary>
public static class RateTableProvider
{
    /// <summary>
    /// Permanent base rates ($/hr) by classification level
    /// These are the minimum legal rates for non-casual employees
    /// Also used as the base for penalty rate calculations for ALL employee types
    /// </summary>
    public static readonly IReadOnlyDictionary<int, decimal> PermanentRates = new Dictionary<
        int,
        decimal
    >
    {
        { 1, 26.55m },
        { 2, 27.16m },
        { 3, 27.58m },
        { 4, 28.12m },
        { 5, 29.27m },
        { 6, 29.70m },
        { 7, 31.19m },
        { 8, 32.45m },
    };

    /// <summary>
    /// Casual rates ($/hr) by classification level
    /// Includes 25% casual loading
    /// </summary>
    public static readonly IReadOnlyDictionary<int, decimal> CasualRates = new Dictionary<
        int,
        decimal
    >
    {
        { 1, 33.19m },
        { 2, 33.95m },
        { 3, 34.48m },
        { 4, 35.15m },
        { 5, 36.59m },
        { 6, 37.13m },
        { 7, 38.99m },
        { 8, 40.56m },
    };

    /// <summary>
    /// Penalty rate multipliers for permanent employees (FullTime, PartTime, FixedTerm)
    /// Applied to Permanent Rate
    /// </summary>
    public static class PermanentMultipliers
    {
        public const decimal Saturday = 1.25m;
        public const decimal Sunday = 1.50m;
        public const decimal PublicHoliday = 2.25m;
    }

    /// <summary>
    /// Penalty rate multipliers for casual employees
    /// Applied to Permanent Rate (NOT Casual Rate)
    /// </summary>
    public static class CasualMultipliers
    {
        public const decimal Saturday = 1.50m;
        public const decimal Sunday = 1.75m;
        public const decimal PublicHoliday = 2.50m;
    }

    /// <summary>
    /// Superannuation Guarantee rate (12%)
    /// Applied to Gross Pay
    /// </summary>
    public const decimal SuperannuationRate = 0.12m;

    /// <summary>
    /// Tolerance for hourly rate comparisons ($0.01)
    /// Used for BaseRate and CasualLoading checks
    /// </summary>
    public const decimal RateTolerance = 0.01m;

    /// <summary>
    /// Tolerance for pay amount comparisons ($0.05)
    /// Used for Penalty and Superannuation checks
    /// </summary>
    public const decimal PayTolerance = 0.05m;

    /// <summary>
    /// Gets the permanent rate for a given level
    /// Returns 0 if level is not found (should trigger pre-validation error)
    /// </summary>
    public static decimal GetPermanentRate(int level)
    {
        return PermanentRates.TryGetValue(level, out var rate) ? rate : 0m;
    }

    /// <summary>
    /// Gets the casual rate for a given level
    /// Returns 0 if level is not found (should trigger pre-validation error)
    /// </summary>
    public static decimal GetCasualRate(int level)
    {
        return CasualRates.TryGetValue(level, out var rate) ? rate : 0m;
    }

    /// <summary>
    /// Parses award level from classification string
    /// Example: "Level 1" -> 1, "Level 2" -> 2
    /// Returns 0 if cannot parse (to indicate invalid classification)
    /// </summary>
    public static int ParseLevel(string classification)
    {
        if (string.IsNullOrWhiteSpace(classification))
            return 0;

        var normalized = classification.Trim().ToLowerInvariant();

        // Try to extract number from "Level X" format
        if (normalized.StartsWith("level"))
        {
            var parts = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2 && int.TryParse(parts[1], out var level))
            {
                return level;
            }
        }

        // Try to parse as direct number
        if (int.TryParse(normalized, out var directLevel))
        {
            return directLevel;
        }

        // Cannot parse
        return 0;
    }
}
