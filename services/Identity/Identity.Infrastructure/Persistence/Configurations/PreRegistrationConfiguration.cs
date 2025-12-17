using Identity.Domain.Entities;
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

        b.OwnsOne(x => x.Email, e =>
        {
            e.Property(p => p.Value).HasColumnName("email").IsRequired();
        });
        b.HasIndex("email").IsUnique();

        b.OwnsOne(x => x.Name, n =>
        {
            n.Property(p => p.FirstName).HasColumnName("first_name").IsRequired();
            n.Property(p => p.MiddleName).HasColumnName("middle_name");
            n.Property(p => p.LastName).HasColumnName("last_name").IsRequired();
        });

        b.Property(x => x.Dob).HasColumnName("dob").IsRequired();
        b.Property(x => x.Gender).HasColumnName("gender");

        b.OwnsOne(x => x.PasswordHash, h =>
        {
            h.Property(p => p.Value).HasColumnName("password_hash").IsRequired();
        });
    }
}