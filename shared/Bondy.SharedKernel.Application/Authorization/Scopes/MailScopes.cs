namespace Bondy.SharedKernel.Application.Authorization.Scopes;

public static class MailScopes
{
    public const string Send = "mail.send";
    public const string TemplatesManage = "mail.templates.manage";
    public const string Read = "mail.read";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        Send, TemplatesManage, Read
    };
}