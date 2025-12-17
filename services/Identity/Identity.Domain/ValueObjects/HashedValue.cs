namespace Identity.Domain.ValueObjects;

public sealed class HashedValue : ValueObject
{
    public string Value { get; }

    private HashedValue(string value)
    {
        Value = value;
    }

    public static HashedValue Create(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("Hash value is required");

        return new HashedValue(hash);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}