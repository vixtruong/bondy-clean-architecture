using Bondy.SharedKernel.Domain.Common;

namespace Identity.Domain.ValueObjects;

public sealed class HashedValue : ValueObject
{
    public string Value { get; }

    private HashedValue(string value) => Value = value;

    public static Result<HashedValue> Create(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            return Result.Failure<HashedValue>(
                Error.Validation(ErrorCodes.Validation.Required, "Hash value is required"));

        hash = hash.Trim();

        return Result.Success(new HashedValue(hash));
    }

    public static HashedValue FromPersisted(string value)
        => Create(value).ValueOrThrow();

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}