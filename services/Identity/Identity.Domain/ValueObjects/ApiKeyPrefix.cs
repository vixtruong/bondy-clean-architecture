namespace Identity.Domain.ValueObjects;

public sealed class ApiKeyPrefix : ValueObject
{
    public string Value { get; }

    private ApiKeyPrefix(string value)
    {
        Value = value;
    }

    public static ApiKeyPrefix Create(string prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            throw new ArgumentException("Prefix is required");

        if (prefix.Length > 12)
            throw new ArgumentException("Prefix max length is 12");

        return new ApiKeyPrefix(prefix.ToLowerInvariant());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}