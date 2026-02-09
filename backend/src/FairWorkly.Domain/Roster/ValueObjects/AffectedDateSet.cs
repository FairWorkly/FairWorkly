using System.Globalization;

namespace FairWorkly.Domain.Roster.ValueObjects;

public readonly record struct AffectedDateSet
{
    private const string DateFormat = "yyyy-MM-dd";

    public static readonly AffectedDateSet Empty = new(null);

    public string? Value { get; init; }

    public AffectedDateSet(string? value)
    {
        Value = Normalize(value);
    }

    public static AffectedDateSet Parse(string? value) => new(value);

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public string? ToStorageString() => IsEmpty ? null : Value;

    public IReadOnlyList<DateOnly> Dates
    {
        get
        {
            if (IsEmpty)
                return Array.Empty<DateOnly>();

            var tokens = Value!.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0)
                return Array.Empty<DateOnly>();

            var dates = new List<DateOnly>(capacity: tokens.Length);
            foreach (var token in tokens)
            {
                if (
                    DateOnly.TryParseExact(
                        token.Trim(),
                        DateFormat,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var date
                    )
                )
                {
                    dates.Add(date);
                }
            }

            return dates;
        }
    }

    public static AffectedDateSet FromDates(IEnumerable<DateTime> dates)
    {
        return FromDates(dates.Select(DateOnly.FromDateTime));
    }

    public static AffectedDateSet FromDates(IEnumerable<DateOnly> dates)
    {
        var set = new SortedSet<DateOnly>();
        foreach (var date in dates)
        {
            set.Add(date);
        }

        if (set.Count == 0)
            return Empty;

        var normalized = string.Join(
            ',',
            set.Select(d => d.ToString(DateFormat, CultureInfo.InvariantCulture))
        );
        return new AffectedDateSet(normalized);
    }

    public AffectedDateSet Add(DateOnly date)
    {
        if (IsEmpty)
            return FromDates([date]);

        return FromDates(Dates.Append(date));
    }

    public AffectedDateSet Union(AffectedDateSet other)
    {
        if (IsEmpty)
            return other;
        if (other.IsEmpty)
            return this;

        return FromDates(Dates.Concat(other.Dates));
    }

    public bool Contains(DateOnly date)
    {
        return Dates.Contains(date);
    }

    public override string ToString() => Value ?? string.Empty;

    private static string? Normalize(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        var set = new SortedSet<DateOnly>();
        var tokens = raw.Split(',', StringSplitOptions.RemoveEmptyEntries);

        foreach (var token in tokens)
        {
            var trimmed = token.Trim();
            if (trimmed.Length == 0)
                continue;

            if (
                DateOnly.TryParseExact(
                    trimmed,
                    DateFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsed
                )
            )
            {
                set.Add(parsed);
            }
        }

        if (set.Count == 0)
            return null;

        return string.Join(
            ',',
            set.Select(d => d.ToString(DateFormat, CultureInfo.InvariantCulture))
        );
    }
}

