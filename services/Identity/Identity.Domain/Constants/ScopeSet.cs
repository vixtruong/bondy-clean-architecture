using Bondy.SharedKernel.Constants.Authorization;
using Identity.Domain.ValueObjects; // Scope

namespace Identity.Domain.Constants;

public static class ScopeSet
{
    // helper
    private static IEnumerable<string> GetScopesByPrefixes(params string[] prefixes)
    {
        if (prefixes.Length == 0) return [];

        var all = Scopes.All;

        return all.Where(s =>
        {
            foreach (var p in prefixes)
            {
                if (p.EndsWith("*"))
                {
                    var trimmed = p.Substring(0, p.Length - 1);
                    if (s.StartsWith(trimmed, StringComparison.OrdinalIgnoreCase)) return true;
                }
                else
                {
                    if (s.StartsWith(p, StringComparison.OrdinalIgnoreCase)) return true;
                }
            }
            return false;
        });
    }

    private static IReadOnlyCollection<Scope> ToScopeCollection(IEnumerable<string> seq) =>
        seq.Distinct(StringComparer.OrdinalIgnoreCase)
           .Select(s => new Scope(s))
           .ToArray();

    // USER
    public static IReadOnlyCollection<Scope> UserScopes =>
        ToScopeCollection(GetScopesByPrefixes("profile.", "auth.", "posts.", "payments."));

    // ADMIN
    public static IReadOnlyCollection<Scope> AdminScopes
    {
        get
        {
            var adminPrefixes = new[] { "admin." };
            var combined = GetScopesByPrefixes(adminPrefixes)
                .Concat(UserScopes.Select(s => s.Value));
            return ToScopeCollection(combined);
        }
    }

    // Partner
    public static IReadOnlyCollection<Scope> PartnerScopes =>
        ToScopeCollection(GetScopesByPrefixes("posts.", "webhook.", "partner."));

    // Internal (wildcard example)
    public static IReadOnlyCollection<Scope> InternalScopes =>
        ToScopeCollection(GetScopesByPrefixes("internal.*"));
}
