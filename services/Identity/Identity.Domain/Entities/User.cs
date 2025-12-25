using Identity.Domain.Enums;
using Identity.Domain.ValueObjects;
using Bondy.SharedKernel.Common;

namespace Identity.Domain.Entities;

public sealed class User : AggregateRoot
{
    public Email Email { get; private set; } = default!;
    public PersonName Name { get; private set; } = default!;
    public string? AvatarUrl { get; private set; }

    public DateTime? Dob { get; private set; }
    public bool? Gender { get; private set; } // giữ đúng DB BOOLEAN; nếu đổi enum thì sửa kiểu + mapping

    public UserRole Role { get; private set; } = UserRole.User;
    public bool Active { get; private set; } = true;

    public int FriendCount { get; private set; } = 0;

    private readonly List<Account> _accounts = new();
    public IReadOnlyCollection<Account> Accounts => _accounts;

    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens;

    private User() { } // EF

    public User(
        Email email,
        PersonName name,
        DateTime createdAt,
        DateTime? dob = null,
        bool? gender = null,
        string? avatarUrl = null)
    {
        Email = email;
        Name = name;
        Dob = dob;
        Gender = gender;
        AvatarUrl = avatarUrl;

        CreatedAt = createdAt;
        Active = true;
        Role = UserRole.User;
        FriendCount = 0;
    }

    public void SetAvatar(string? avatarUrl, DateTime utcNow)
    {
        AvatarUrl = avatarUrl;
        UpdatedAt = utcNow;
    }

    public void SetProfile(PersonName name, DateTime? dob, bool? gender, DateTime utcNow)
    {
        Name = name;
        Dob = dob;
        Gender = gender;
        UpdatedAt = utcNow;
    }

    public void Deactivate(DateTime utcNow)
    {
        Active = false;
        UpdatedAt = utcNow;
    }

    public void Activate(DateTime utcNow)
    {
        Active = true;
        UpdatedAt = utcNow;
    }

    public void PromoteToAdmin(DateTime utcNow)
    {
        Role = UserRole.Admin;
        UpdatedAt = utcNow;
    }

    public void IncreaseFriendCount(DateTime utcNow)
    {
        FriendCount++;
        UpdatedAt = utcNow;
    }

    public void DecreaseFriendCount(DateTime utcNow)
    {
        if (FriendCount <= 0) return;
        FriendCount--;
        UpdatedAt = utcNow;
    }
}
