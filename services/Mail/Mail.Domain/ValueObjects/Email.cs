using Bondy.SharedKernel.Domain.Common;
using System.Text.RegularExpressions;

namespace Mail.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Result<Email> Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Result.Failure<Email>(Error.Validation(ErrorCodes.Validation.Required, "Email is required"));

        input = input.Trim().ToLowerInvariant();

        if (!Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return Result.Failure<Email>(Error.Validation(ErrorCodes.Validation.InvalidFormat, "Invalid email format"));

        return Result.Success(new Email(input));
    }

    public static Email FromPersisted(string value)
    {
        var r = Create(value);
        return r.ValueOrThrow();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}