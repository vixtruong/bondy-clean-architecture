namespace Identity.Contracts.Mail;

public record MailSendResult(bool Success, int StatusCode, object? Data);