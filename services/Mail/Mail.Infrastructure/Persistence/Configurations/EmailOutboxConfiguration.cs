using Mail.Domain.Entities;
using Mail.Infrastructure.Persistence.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mail.Infrastructure.Persistence.Configurations;

public sealed class EmailOutboxConfiguration : IEntityTypeConfiguration<EmailOutbox>
{
    public void Configure(EntityTypeBuilder<EmailOutbox> builder)
    {
        builder.ToTable("email_outboxes");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Purpose)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Subject)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Html)
            .HasColumnType("text");

        builder.Property(x => x.PayloadJson)
            .HasColumnName("payload_json")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.DedupKey)
            .HasColumnName("dedup_key")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.AttemptCount)
            .HasColumnName("attempt_count")
            .IsRequired();

        builder.Property(x => x.LastAttemptAt)
            .HasColumnName("last_attempt_at");

        builder.Property(x => x.SentAt)
            .HasColumnName("sent_at");

        builder.OwnsOne(x => x.To, b =>
        {
            b.Property(p => p.Value)
                .HasColumnName("to_email")
                .HasMaxLength(320)
                .IsRequired();
        });

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasConversion(Converter.UtcConverter)
            .IsRequired();

        builder.Ignore(x => x.UpdatedAt);
    }
}
