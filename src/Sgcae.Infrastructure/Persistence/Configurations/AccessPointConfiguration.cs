using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sgcae.Domain.AccessPoints;

namespace Sgcae.Infrastructure.Persistence.Configurations;

internal sealed class AccessPointConfiguration : IEntityTypeConfiguration<AccessPoint>
{
    public void Configure(EntityTypeBuilder<AccessPoint> builder)
    {
        builder.ToTable("AccessPoints");
        builder.HasKey(accessPoint => accessPoint.Id);
        builder.Property(accessPoint => accessPoint.Id).ValueGeneratedNever();

        builder.Property(accessPoint => accessPoint.Code)
            .HasConversion(code => code.Value, value => AccessPointCode.Create(value))
            .HasMaxLength(AccessPointCode.MaxLength)
            .IsRequired();
        builder.HasIndex(accessPoint => accessPoint.Code).IsUnique();

        builder.Property(accessPoint => accessPoint.Name).HasMaxLength(AccessPoint.NameMaxLength).IsRequired();
        builder.Property(accessPoint => accessPoint.Location).HasMaxLength(AccessPoint.LocationMaxLength).IsRequired();
        builder.Property(accessPoint => accessPoint.Kind).HasConversion<string>().HasMaxLength(30);
        builder.Property(accessPoint => accessPoint.RegisteredAt);

        builder.HasIndex(accessPoint => accessPoint.IsEnabled);
    }
}
