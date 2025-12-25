namespace Identity.Api.Http;

public static class CookieExtensions
{
    public static void SetRefreshInfoCookie(
        this HttpResponse response,
        HttpRequest request,
        long userId,
        string refreshTokenRaw,
        int days,
        string path)
    {
        var isHttps = request.IsHttps;

        var sameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax;

        response.Cookies.Append("rt", refreshTokenRaw, new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = sameSite,
            Path = path,
            Expires = DateTimeOffset.UtcNow.AddDays(days)
        });

        response.Cookies.Append("uid", userId.ToString(), new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = sameSite,
            Path = path,
            Expires = DateTimeOffset.UtcNow.AddDays(days)
        });
    }

    public static void ClearRefreshTokenCookie(
        this HttpResponse response,
        string path)
    {
        response.Cookies.Delete("rt", new CookieOptions
        {
            Path = path,
            SameSite = SameSiteMode.None
        });
    }
}