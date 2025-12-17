using Identity.Domain.ValueObjects;
using Bondy.SharedKernel.Common;

namespace Identity.Domain.Entities;

public sealed class PreRegistration : AggregateRoot
{
    public Email Email { get; private set; } = default!;
    public PersonName Name { get; private set; } = default!;
    public DateTime Dob { get; private set; }
    public bool? Gender { get; private set; } // giữ đúng DB

    public HashedValue PasswordHash { get; private set; } = default!;

    private PreRegistration() { }

    public PreRegistration(
        Email email,
        PersonName name,
        DateTime dob,
        bool? gender,
        HashedValue passwordHash,
        DateTime createdAtUtc)
    {
        Email = email;
        Name = name;
        Dob = dob;
        Gender = gender;
        PasswordHash = passwordHash;
        CreatedAt = createdAtUtc;
    }
}