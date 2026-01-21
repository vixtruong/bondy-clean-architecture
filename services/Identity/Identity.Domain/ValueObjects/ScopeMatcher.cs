namespace Identity.Domain.ValueObjects;

public static class ScopeMatcher
{
    // Simple wildcard match: '*' matches any suffix. e.g. "admin:*" matches "admin.users.read"
    // Pattern and scope assumed normalized (lowercase).
    public static bool IsMatch(string pattern, string scope)
    {
        if (string.Equals(pattern, scope, StringComparison.OrdinalIgnoreCase)) return true;
        if (string.IsNullOrEmpty(pattern) || string.IsNullOrEmpty(scope)) return false;

        var p = pattern.ToLowerInvariant();
        var s = scope.ToLowerInvariant();

        if (!p.Contains('*')) return false;

        var prefix = p.Split('*', 2)[0];
        return s.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
    }
}