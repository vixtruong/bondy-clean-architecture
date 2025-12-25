using Bondy.SharedKernel.Common;
using Mail.Domain.Enums;
using Mail.Domain.ValueObjects;

namespace Mail.Domain.Entities;

public class EmailLog : AggregateRoot
{
    public EmailPurpose Purpose { get; private set; }

    public Email To { get; private set; } = default!;

    public MailStatus Status { get; private set; }

    public DateTime? SentAt { get; private set; }

    private EmailLog() { }

    public EmailLog(EmailPurpose purpose, Email to, DateTime now)
    {
        Purpose = purpose;
        To = to;
        Status = MailStatus.Pending;
        CreatedAt = now;
    }

    public void MarkSent(DateTime now)
    {
        Status = MailStatus.Sent;
        SentAt = now;
    }

    public void MarkFailed() => Status = MailStatus.Failed;
}
