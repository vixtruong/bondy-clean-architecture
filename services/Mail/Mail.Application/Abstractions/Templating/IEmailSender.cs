namespace Mail.Application.Abstractions.Templating;

public interface IEmailSender
{
    Task SendHtmlAsync(string to, string subject, string html);
}
