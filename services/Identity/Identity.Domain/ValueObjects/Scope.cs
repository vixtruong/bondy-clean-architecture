namespace Identity.Domain.ValueObjects;

public sealed record Scope
{
    public string Value { get; init; }

    public Scope(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Scope cannot be empty.", nameof(value));

        Value = value.Trim().ToLowerInvariant();
    }

    public override string ToString() => Value;
}