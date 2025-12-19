using Identity.Domain.Enums;
using Identity.Domain.ValueObjects;
using Bondy.SharedKernel.Common;

namespace Identity.Domain.Entities;

public sealed class OtpCode : AggregateRoot
{
    public OtpSubjectType SubjectType { get; private set; }
    public long SubjectId { get; private set; }

    public OtpPurpose Purpose { get; private set; }

    public HashedValue CodeHash { get; private set; } = default!;
    public DateTime ExpiresAt { get; private set; }

    public int Attempts { get; private set; } = 0;
    public bool Active { get; private set; } = true;

    private OtpCode() { }

    public OtpCode(
        OtpSubjectType subjectType,
        long subjectId,
        OtpPurpose purpose,
        HashedValue codeHash,
        DateTime expiresAtUtc,
        DateTime createdAtUtc)
    {
        SubjectType = subjectType;
        SubjectId = subjectId;
        Purpose = purpose;
        CodeHash = codeHash;
        ExpiresAt = expiresAtUtc;

        Attempts = 0;
        Active = true;
        CreatedAt = createdAtUtc;
    }

    public bool IsExpired(DateTime utcNow) => utcNow >= ExpiresAt;

    public void IncreaseAttempts(DateTime utcNow)
    {
        Attempts++;
        UpdatedAt = utcNow;
    }

    public void Deactivate(DateTime utcNow)
    {
        Active = false;
        UpdatedAt = utcNow;
    }
}