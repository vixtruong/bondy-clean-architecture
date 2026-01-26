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

    // ---------- OAuth2-specific helpers ----------

    /// <summary>Set short-lived access token in HttpOnly cookie (name "at").</summary>
    public static void SetAccessTokenCookie(this HttpResponse response, HttpRequest request, string accessToken, int minutes)
    {
        var isHttps = request.IsHttps;
        var sameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax;

        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = sameSite,
            Path = DefaultPath,
            Expires = DateTimeOffset.UtcNow.AddMinutes(minutes)
        };

        response.Cookies.Append("at", accessToken, options);
    }

    /// <summary>Clear access token cookie.</summary>
    public static void ClearAccessTokenCookie(this HttpResponse response, HttpRequest request)
    {
        var isHttps = request.IsHttps;
        var sameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax;

        var options = new CookieOptions
        {
            Path = DefaultPath,
            Secure = isHttps,
            SameSite = sameSite
        };

        response.Cookies.Delete("at", options);
    }

    /// <summary>Set a transient oauth_state cookie (used for CSRF/state validation).</summary>
    public static void SetOAuthStateCookie(this HttpResponse response, HttpRequest request, string state, int minutes = 10)
    {
        var isHttps = request.IsHttps;
        var sameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax;

        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = sameSite,
            Path = DefaultPath,
            Expires = DateTimeOffset.UtcNow.AddMinutes(minutes)
        };

        response.Cookies.Append("oauth_state", state, options);
    }

    /// <summary>Try read oauth_state cookie value from the request.</summary>
    public static bool TryGetOAuthState(this HttpRequest request, out string? state)
    {
        state = null;
        if (request.Cookies == null) return false;
        if (!request.Cookies.TryGetValue("oauth_state", out var s)) return false;
        state = s;
        return true;
    }

    /// <summary>Clear oauth_state cookie.</summary>
    public static void ClearOAuthStateCookie(this HttpResponse response, HttpRequest request)
    {
        var isHttps = request.IsHttps;
        var sameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax;

        var options = new CookieOptions
        {
            Path = DefaultPath,
            Secure = isHttps,
            SameSite = sameSite
        };

        response.Cookies.Delete("oauth_state", options);
    }
}