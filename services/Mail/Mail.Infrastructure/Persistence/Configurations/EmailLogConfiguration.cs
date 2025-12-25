using Mail.Domain.Entities;
using Mail.Infrastructure.Persistence.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mail.Infrastructure.Persistence.Configurations;

public sealed class EmailLogConfiguration : IEntityTypeConfiguration<EmailLog>
{
    public void Configure(EntityTypeBuilder<EmailLog> builder)
    {
        builder.ToTable("email_logs");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Purpose)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.SentAt);

        builder.OwnsOne(x => x.To, b =>
        {
            b.Property(p => p.Value)
                .HasColumnName("to_email")
                .HasMaxLength(320)
                .IsRequired();
        });

        builder.Ignore(x => x.UpdatedAt);

        builder.Property(x => x.CreatedAt)
            .HasConversion(Converter.UtcConverter)
            .IsRequired();
    }
}