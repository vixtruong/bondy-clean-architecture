// Identity.Domain.Entities.Role.cs
using Identity.Domain.ValueObjects;
using Bondy.SharedKernel.Domain.Common;

namespace Identity.Domain.Entities;

public sealed class Role : AggregateRoot
{
    public string Code { get; private set; } = default!; // "admin", "editor"
    public string Name { get; private set; } = default!;

    private readonly List<Scope> _scopes = new();
    public IReadOnlyCollection<Scope> Scopes => _scopes;

    //private readonly List<User> _users = new();
    //public IReadOnlyCollection<User> Users => _users;

    private Role() { } // EF

    public Role(string code, string name, IEnumerable<Scope>? scopes = null, DateTime? createdAt = null)
    {
        Code = code;
        Name = name;
        CreatedAt = createdAt ?? DateTime.UtcNow;
        if (scopes != null) _scopes.AddRange(scopes);
    }

    public void AddScope(Scope scope)
    {
        if (_scopes.Any(s => s.Value == scope.Value)) return;
        _scopes.Add(scope);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveScope(string scope)
    {
        _scopes.RemoveAll(s => s.Value == scope);
        UpdatedAt = DateTime.UtcNow;
    }
}