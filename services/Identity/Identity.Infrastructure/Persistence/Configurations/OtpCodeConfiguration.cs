using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public sealed class OtpCodeConfiguration : IEntityTypeConfiguration<OtpCode>
{
    public void Configure(EntityTypeBuilder<OtpCode> b)
    {
        b.ToTable("otp_codes");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id");

        b.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        // subject_type VARCHAR(20)
        b.Property(x => x.SubjectType)
            .HasColumnName("subject_type")
            .HasConversion(
                v => v.ToString().ToUpperInvariant(),
                v => Enum.Parse<OtpSubjectType>(v, true))
            .HasMaxLength(20)
            .IsRequired();

        b.Property(x => x.SubjectId).HasColumnName("subject_id").IsRequired();

        // purpose VARCHAR(20)
        b.Property(x => x.Purpose)
            .HasColumnName("purpose")
            .HasConversion(
                v => v.ToString().ToUpperInvariant(),
                v => Enum.Parse<OtpPurpose>(v, true))
            .HasMaxLength(20)
            .IsRequired();

        b.OwnsOne(x => x.CodeHash, h =>
        {
            h.Property(p => p.Value).HasColumnName("code_hash").IsRequired();
        });

        b.Property(x => x.ExpiresAt).HasColumnName("expires_at").IsRequired();
        b.Property(x => x.Attempts).HasColumnName("attempts").IsRequired();
        b.Property(x => x.Active).HasColumnName("active").IsRequired();

        // Postgres partial unique index:
        b.HasIndex(x => new { x.SubjectId, x.Purpose })
            .IsUnique()
            .HasFilter("active = TRUE")
            .HasDatabaseName("ux_active_otp_per_pre_reg");
    }
}
