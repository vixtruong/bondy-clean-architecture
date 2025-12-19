using Mail.Domain.Enums;

namespace Mail.Application.Templating;

public sealed record TemplateSpec(
    string FileName,
    string Subject,
    string[] RequiredKeys);

public static class TemplateDefinitions
{
    public static readonly TemplateSpec Layout =
        new(FileName: "_layout.html", Subject: "", RequiredKeys: Array.Empty<string>());

    public static readonly TemplateSpec Welcome =
        new(FileName: "welcome.html", Subject: "Welcome to Bondy", RequiredKeys: new[] { "firstName", "email" });

    public static readonly TemplateSpec OAuth2Welcome =
        new(FileName: "oauth2_welcome.html",
            Subject: "Your Bondy account details",
            RequiredKeys: new[] { "firstName", "provider", "email", "password" });

    public static readonly TemplateSpec Registration =
        new(FileName: "otp_registration.html",
            Subject: "Your Bondy verification code",
            RequiredKeys: new[] { "firstName", "otp", "expiresMinutes" });

    public static readonly TemplateSpec ResetPassword =
        new(FileName: "reset_password_otp.html",
            Subject: "Reset your Bondy password",
            RequiredKeys: new[] { "firstName", "otp", "expiresMinutes" });

    public static TemplateSpec For(EmailPurpose purpose) => purpose switch
    {
        EmailPurpose.Welcome => Welcome,
        EmailPurpose.OAuth2Welcome => OAuth2Welcome,
        EmailPurpose.Registration => Registration,
        EmailPurpose.ResetPassword => ResetPassword,
        _ => throw new ArgumentOutOfRangeException(nameof(purpose), purpose, null)
    };
}