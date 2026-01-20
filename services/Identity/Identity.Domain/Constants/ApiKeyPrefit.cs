namespace Identity.Domain.Constants;

public static class ApiKeyPrefix
{
    // system
    public const string System = "bondy";

    // environment
    public const string Live = "live";
    public const string Test = "test";

    // client type
    public const string Internal = "internal";
    public const string App = "app";
    public const string Partner = "partner";
    public const string Webhook = "webhook";

    public static string Build(
        string environment,
        string clientType,
        string shortId)
    {
        return $"{System}_{environment}_{clientType}_{shortId}";
    }
}