namespace Identity.Infrastructure.Integrations.Mail;

public sealed class MailClientOptions
{
    public const string SectionName = "Clients:Mail";
    public string BaseUrl { get; init; } = default!;
}