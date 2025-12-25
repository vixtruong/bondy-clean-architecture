namespace Identity.Infrastructure.Common.Security;

public sealed class JwtOptions
{
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public string Secret { get; init; } = default!;
    public int AccessTokenMinutes { get; init; } = 15;
}