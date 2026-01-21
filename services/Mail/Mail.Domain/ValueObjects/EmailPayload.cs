using Bondy.SharedKernel.Domain.Common;

namespace Mail.Domain.ValueObjects;

public class EmailPayload : ValueObject
{
    public string Json { get; private set; } = null!;

    private EmailPayload() { }

    public EmailPayload(string json)
    {
        Json = json;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Json;
    }

    public static Result<EmailPayload> Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Result.Failure<EmailPayload>(Error.Validation(ErrorCodes.Validation.Required, "Email payload is required"));

        input = input.Trim().ToLowerInvariant();

        return Result.Success(new EmailPayload(input));
    }

    public static EmailPayload FromPersisted(string value)
    {
        var r = Create(value);
        return r.ValueOrThrow();
    }
}