using FairWorkly.Domain.Roster.Enums;

namespace FairWorkly.Domain.Roster.ValueObjects;

public readonly record struct ExecutedCheckTypeSet
{
    public static readonly ExecutedCheckTypeSet Empty = new(null);

    public string? Value { get; init; }

    public ExecutedCheckTypeSet(string? value)
    {
        Value = Normalize(value);
    }

    public static ExecutedCheckTypeSet Parse(string? value) => new(value);

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public string? ToStorageString() => IsEmpty ? null : Value;

    public IReadOnlyList<RosterCheckType> CheckTypes
    {
        get
        {
            if (IsEmpty)
                return Array.Empty<RosterCheckType>();

            var tokens = Value!.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0)
                return Array.Empty<RosterCheckType>();

            var parsed = new List<RosterCheckType>(capacity: tokens.Length);
            var seen = new HashSet<RosterCheckType>();

            foreach (var token in tokens)
            {
                var trimmed = token.Trim();
                if (trimmed.Length == 0)
                    continue;

                if (!Enum.TryParse<RosterCheckType>(trimmed, ignoreCase: false, out var checkType))
                    continue;

                if (seen.Add(checkType))
                    parsed.Add(checkType);
            }

            return parsed;
        }
    }

    public static ExecutedCheckTypeSet FromCheckTypes(IEnumerable<RosterCheckType> checkTypes)
    {
        var types = checkTypes.Distinct().OrderBy(c => (int)c).ToArray();
        if (types.Length == 0)
            return Empty;

        return new ExecutedCheckTypeSet(string.Join(',', types.Select(t => t.ToString())));
    }

    public ExecutedCheckTypeSet Add(RosterCheckType checkType)
    {
        if (IsEmpty)
            return FromCheckTypes([checkType]);

        return FromCheckTypes(CheckTypes.Append(checkType));
    }

    public ExecutedCheckTypeSet Union(ExecutedCheckTypeSet other)
    {
        if (IsEmpty)
            return other;
        if (other.IsEmpty)
            return this;

        return FromCheckTypes(CheckTypes.Concat(other.CheckTypes));
    }

    public bool Contains(RosterCheckType checkType)
    {
        return CheckTypes.Contains(checkType);
    }

    public override string ToString() => Value ?? string.Empty;

    private static string? Normalize(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        var set = new HashSet<RosterCheckType>();
        var tokens = raw.Split(',', StringSplitOptions.RemoveEmptyEntries);

        foreach (var token in tokens)
        {
            var trimmed = token.Trim();
            if (trimmed.Length == 0)
                continue;

            if (!Enum.TryParse<RosterCheckType>(trimmed, ignoreCase: false, out var parsed))
                continue;

            set.Add(parsed);
        }

        if (set.Count == 0)
            return null;

        return string.Join(',', set.OrderBy(c => (int)c).Select(c => c.ToString()));
    }
}

