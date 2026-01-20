namespace Identity.Domain.Constants;

public static class TokenPolicy
{
    public const int RefreshTokenByteLength = 64;
    public const int RefreshTokenDays = 15;
}

public static class OtpPolicy
{
    public const int Length = 6;
    public const int ExpiryMinutes = 30;
    public const int MaxAttempts = 5;
}

public static class ApiKeyPolicy
{
    public static readonly TimeSpan DefaultGracePeriod =
        TimeSpan.FromMinutes(30);
}