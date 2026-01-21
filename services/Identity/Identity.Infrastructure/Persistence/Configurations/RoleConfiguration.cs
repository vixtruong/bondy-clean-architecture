// Identity.Infrastructure/Persistence/Configurations/RoleConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Identity.Domain.Entities;
using Identity.Domain.ValueObjects;

namespace Identity.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> b)
    {
        b.ToTable("roles");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id");

        b.Property(x => x.Code).HasColumnName("code").HasMaxLength(100).IsRequired();
        b.HasIndex(x => x.Code).IsUnique();

        b.Property(x => x.Name).HasColumnName("name").HasMaxLength(200).IsRequired();

        b.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        b.OwnsMany(x => x.Scopes, sb =>
        {
            sb.ToTable("role_scopes");
            sb.WithOwner().HasForeignKey("role_id");
            sb.Property(s => s.Value).HasColumnName("scope").HasMaxLength(500).IsRequired();
            sb.HasKey("role_id", nameof(Scope.Value));
            sb.HasIndex("role_id");
        });
    }
}