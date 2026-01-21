// Identity.Domain.Entities.User.cs (cập nhật)
using Bondy.SharedKernel.Domain.Common;
using Identity.Domain.Enums;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.Entities;

public sealed class User : AggregateRoot
{
    public Email Email { get; private set; } = default!;
    public PersonName Name { get; private set; } = default!;
    public string? AvatarUrl { get; private set; }

    public DateTime? Dob { get; private set; }
    public bool? Gender { get; private set; }

    public bool Active { get; private set; } = true;

    private readonly List<Role> _roles = new();
    public IReadOnlyCollection<Role> Roles => _roles;

    private readonly List<Scope> _grantedScopes = new();
    public IReadOnlyCollection<Scope> GrantedScopes => _grantedScopes;

    private readonly List<Scope> _deniedScopes = new();
    public IReadOnlyCollection<Scope> DeniedScopes => _deniedScopes;

    private readonly List<Account> _accounts = new();
    public IReadOnlyCollection<Account> Accounts => _accounts;

    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens;

    private User() { }

    public User(Email email, PersonName name, 
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
        Active = true;
        CreatedAt = createdAt;
    }

    // Role operations
    public void AssignRole(Role role)
    {
        if (role == null)
            throw new ArgumentNullException(nameof(role));

        if (_roles.Any(r => r.Code == role.Code))
            return;

        _roles.Add(role);
        UpdatedAt = DateTime.UtcNow;
    }


    public void RemoveRole(string roleCode)
    {
        _roles.RemoveAll(r => r.Code == roleCode);
        UpdatedAt = DateTime.UtcNow;
    }

    // Grants / denies
    public void GrantScope(Scope scope)
    {
        if (_grantedScopes.Any(s => s.Value == scope.Value)) return;
        _grantedScopes.Add(scope);
        _deniedScopes.RemoveAll(d => d.Value == scope.Value);
        UpdatedAt = DateTime.UtcNow;
    }

    public void DenyScope(Scope scope)
    {
        if (_deniedScopes.Any(s => s.Value == scope.Value)) return;
        _deniedScopes.Add(scope);
        _grantedScopes.RemoveAll(g => g.Value == scope.Value);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RevokeGrantedScope(string scope)
    {
        _grantedScopes.RemoveAll(s => s.Value == scope);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveDeniedScope(string scope)
    {
        _deniedScopes.RemoveAll(s => s.Value == scope);
        UpdatedAt = DateTime.UtcNow;
    }

    // Effective check
    public bool HasScope(string requiredScope)
    {
        // denied takes precedence
        if (_deniedScopes.Any(d => ScopeMatcher.IsMatch(d.Value, requiredScope) || string.Equals(d.Value, requiredScope, StringComparison.OrdinalIgnoreCase)))
            return false;

        if (_grantedScopes.Any(g => ScopeMatcher.IsMatch(g.Value, requiredScope) || string.Equals(g.Value, requiredScope, StringComparison.OrdinalIgnoreCase)))
            return true;

        if (_roles.Any(r => r.Scopes.Any(s => ScopeMatcher.IsMatch(s.Value, requiredScope) || string.Equals(s.Value, requiredScope, StringComparison.OrdinalIgnoreCase))))
            return true;

        return false;
    }

    public IEnumerable<string> GetEffectiveScopes()
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var r in _roles)
            foreach (var s in r.Scopes)
                set.Add(s.Value);

        foreach (var g in _grantedScopes)
            set.Add(g.Value);

        // remove denies (consider wildcard denies)
        foreach (var d in _deniedScopes)
        {
            var denies = set.Where(x => ScopeMatcher.IsMatch(d.Value, x) || string.Equals(x, d.Value, StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (var rem in denies) set.Remove(rem);
        }

        return set;
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
