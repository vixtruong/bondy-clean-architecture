namespace Identity.Application.Results.Mail;

public record MailSendResult(bool Success, int StatusCode, object? Data);