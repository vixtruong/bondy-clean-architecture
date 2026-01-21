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
    public bool? Gender { get; private set; }

    public UserRole Role { get; private set; } = UserRole.User;
    public bool Active { get; private set; } = true;

    public int FriendCount { get; private set; }

    private readonly List<Scope> _scopes = new();
    public IReadOnlyCollection<Scope> Scopes => _scopes;

    private readonly List<Account> _accounts = new();
    public IReadOnlyCollection<Account> Accounts => _accounts;

    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens;

    private User() { }

    public User(
        Email email,
        PersonName name,
        IEnumerable<Scope> scopes,
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

        Role = UserRole.User;
        Active = true;
        CreatedAt = createdAt;

        AssignScopes(scopes);
    }

    private void AssignScopes(IEnumerable<Scope> scopes)
    {
        foreach (var scope in scopes)
        {
            if (_scopes.Any(s => s.Value == scope.Value))
                continue;

            _scopes.Add(scope);
        }
    }


    public bool HasScope(string scope)
        => _scopes.Any(s => s.Value == scope);

    public void GrantScope(Scope scope, DateTime utcNow)
    {
        if (_scopes.Any(s => s.Value == scope.Value)) return;
        _scopes.Add(scope);
        UpdatedAt = utcNow;
    }

    public void RevokeScope(string scope, DateTime utcNow)
    {
        _scopes.RemoveAll(s => s.Value == scope);
        UpdatedAt = utcNow;
    }

    public void PromoteToAdmin(DateTime utcNow)
    {
        Role = UserRole.Admin;
        GrantScope(new Scope("admin:*"), utcNow);
    }

    public void AddLocalAccount(
        HashedValue passwordHash,
        DateTime utcNow)
    {
        if (_accounts.Any(a => a.Provider == AuthProvider.Local))
            return;

        _accounts.Add(new Account(
            AuthProvider.Local,
            passwordHash,
            utcNow));

        UpdatedAt = utcNow;
    }

    public void AddSocialAccount(
        AuthProvider provider,
        DateTime utcNow)
    {
        if (_accounts.Any(a => a.Provider == provider))
            return;

        _accounts.Add(new Account(
            provider,
            passwordHash: null,
            utcNow));

        UpdatedAt = utcNow;
    }

    public bool HasAccount(AuthProvider provider)
    {
        return _accounts.Any(a => a.Provider == provider);
    }
}