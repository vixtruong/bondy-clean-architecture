using Bondy.SharedKernel.Domain.Common;

namespace Identity.Domain.ValueObjects;

public sealed class PersonName : ValueObject
{
    public string FirstName { get; }
    public string? MiddleName { get; }
    public string? LastName { get; }

    private PersonName(string firstName, string? middleName, string? lastName)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
    }

    public static Result<PersonName> Create(string firstName, string? middleName, string? lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure<PersonName>(
                Error.Validation(ErrorCodes.Validation.Required, "First name is required"));

        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Failure<PersonName>(
                Error.Validation(ErrorCodes.Validation.Required, "Last name is required"));

        firstName = firstName.Trim();
        lastName = string.IsNullOrWhiteSpace(lastName) ? null : lastName.Trim();

        middleName = string.IsNullOrWhiteSpace(middleName) ? null : middleName.Trim();

        return Result.Success(new PersonName(firstName, middleName, lastName));
    }

    public static PersonName FromPersisted(string firstName, string? middleName, string? lastName)
        => Create(firstName, middleName, lastName).ValueOrThrow();

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FirstName;
        yield return MiddleName;
        yield return LastName;
    }

    public override string ToString()
        => string.Join(" ", new[] { FirstName, MiddleName, LastName }.Where(x => !string.IsNullOrWhiteSpace(x)));
}