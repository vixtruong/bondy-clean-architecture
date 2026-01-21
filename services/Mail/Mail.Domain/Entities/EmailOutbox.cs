using Bondy.SharedKernel.Domain.Common;
using Mail.Domain.Enums;
using Mail.Domain.ValueObjects;

namespace Mail.Domain.Entities;

public class EmailOutbox : AggregateRoot
{
    public EmailPurpose Purpose { get; private set; }
    public Email To { get; private set; } = default!;
    public string Subject { get; private set; } = default!;
    public string? Html { get; private set; }
    public string PayloadJson { get; private set; } = default!;
    public string DedupKey { get; private set; } = default!;
    public MailStatus Status { get; private set; }
    public int AttemptCount { get; private set; }
    public DateTime? LastAttemptAt { get; private set; }
    public DateTime? SentAt { get; private set; }

    private EmailOutbox() { }

    public EmailOutbox(EmailPurpose purpose, Email to, string subject, string payloadJson, string html, string dedupKey, DateTime now)
    {
        Purpose = purpose;
        To = to;
        Subject = subject;
        PayloadJson = payloadJson;
        Html = html;
        DedupKey = dedupKey;
        Status = MailStatus.Pending;
        CreatedAt = now;
    }

    public void MarkSending(DateTime now) { Status = MailStatus.Sending; AttemptCount++; LastAttemptAt = now; }
    public void MarkSent(string providerId, DateTime now) { Status = MailStatus.Sent; SentAt = now; }
    public void MarkFailed(string? err = null) { Status = MailStatus.Failed; }
}