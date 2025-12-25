namespace Mail.Infrastructure.Templating;

public sealed class SmtpOptions
{
    public const string SectionName = "Mail:Smtp";

    public string Host { get; init; } = default!;
    public int Port { get; init; } = 587;

    public string Username { get; init; } = default!;
    public string Password { get; init; } = default!;

    public string FromEmail { get; init; } = default!;
    public string FromName { get; init; } = "Bondy";

    /// <summary>
    /// true = dùng STARTTLS nếu server support. false = không TLS.
    /// (Nếu bạn dùng SMTPS port 465 thì set UseSsl=true)
    /// </summary>
    public bool UseStartTls { get; init; } = true;

    /// <summary>
    /// true = SSL ngay khi connect (SMTPS, thường port 465)
    /// </summary>
    public bool UseSsl { get; init; } = false;
}