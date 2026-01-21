namespace Identity.Api.Http;

public static class CookieExtensions
{
    private const string DefaultPath = "/";

    public static void SetRefreshInfoCookie(
        this HttpResponse response,
        HttpRequest request,
        long userId,
        string refreshTokenRaw,
        string sessionId,
        int days)
    {
        var isHttps = request.IsHttps;
        var sameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax;

        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = sameSite,
            Path = DefaultPath,
            Expires = DateTimeOffset.UtcNow.AddDays(days)
        };

        response.Cookies.Append("rt", refreshTokenRaw, options);
        response.Cookies.Append("uid", userId.ToString(), options);
        response.Cookies.Append("sessionId", sessionId, options);
    }

    public static void ClearRefreshInfoCookies(
        this HttpResponse response,
        HttpRequest request)
    {
        var isHttps = request.IsHttps;
        var sameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax;

        var options = new CookieOptions
        {
            Path = DefaultPath,
            Secure = isHttps,
            SameSite = sameSite
        };

        response.Cookies.Delete("rt", options);
        response.Cookies.Delete("uid", options);
        response.Cookies.Delete("sessionId", options);
    }
}