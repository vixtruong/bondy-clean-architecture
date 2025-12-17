namespace Identity.Domain.ValueObjects;

public sealed class PersonName : ValueObject
{
    public string FirstName { get; }
    public string? MiddleName { get; }
    public string LastName { get; }

    private PersonName(string firstName, string? middleName, string lastName)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
    }

    public static PersonName Create(string firstName, string? middleName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required");

        return new PersonName(
            firstName.Trim(),
            middleName?.Trim(),
            lastName.Trim()
        );
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FirstName;
        yield return MiddleName;
        yield return LastName;
    }

    public override string ToString()
        => $"{FirstName} {MiddleName} {LastName}".Replace("  ", " ");
}