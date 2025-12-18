using Identity.Domain.Entities;
using Identity.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public sealed class PreRegistrationConfiguration : IEntityTypeConfiguration<PreRegistration>
{
    public void Configure(EntityTypeBuilder<PreRegistration> b)
    {
        b.ToTable("pre_registrations");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id");

        b.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        // Email as scalar column
        b.Property(x => x.Email)
            .HasConversion(v => v.Value, v => Email.Create(v))
            .HasColumnName("email")
            .IsRequired();

        b.HasIndex(x => x.Email)
            .IsUnique()
            .HasDatabaseName("ux_pre_registrations_email");

        // Name as owned (nhiều cột)
        b.OwnsOne(x => x.Name, n =>
        {
            n.Property(p => p.FirstName).HasColumnName("first_name").IsRequired();
            n.Property(p => p.MiddleName).HasColumnName("middle_name");
            n.Property(p => p.LastName).HasColumnName("last_name").IsRequired();
        });

        b.Property(x => x.Dob).HasColumnName("dob").IsRequired();
        b.Property(x => x.Gender).HasColumnName("gender");

        // PasswordHash as scalar column
        b.Property(x => x.PasswordHash)
            .HasConversion(v => v.Value, v => HashedValue.Create(v))
            .HasColumnName("password_hash")
            .IsRequired();
    }
}