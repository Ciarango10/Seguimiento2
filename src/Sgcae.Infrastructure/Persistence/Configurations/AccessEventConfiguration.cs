using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sgcae.Domain.AccessControl;
using Sgcae.Domain.AccessPoints;
using Sgcae.Domain.Credentials;
using Sgcae.Domain.Employees;

namespace Sgcae.Infrastructure.Persistence.Configurations;

internal sealed class AccessEventConfiguration : IEntityTypeConfiguration<AccessEvent>
{
    public void Configure(EntityTypeBuilder<AccessEvent> builder)
    {
        builder.ToTable("AccessEvents");
        builder.HasKey(accessEvent => accessEvent.Id);
        builder.Property(accessEvent => accessEvent.Id).ValueGeneratedNever();

        builder.Property(accessEvent => accessEvent.PresentedCredentialCode)
            .HasMaxLength(AccessEvent.PresentedCredentialCodeMaxLength)
            .IsRequired();
        builder.Property(accessEvent => accessEvent.PresentedAccessPointCode)
            .HasMaxLength(AccessEvent.PresentedAccessPointCodeMaxLength)
            .IsRequired();

        builder.Property(accessEvent => accessEvent.Direction).HasConversion<string>().HasMaxLength(10);
        builder.Property(accessEvent => accessEvent.Result).HasConversion<string>().HasMaxLength(10);
        builder.Property(accessEvent => accessEvent.DenialReason).HasConversion<string>().HasMaxLength(30);

        // Las referencias son opcionales: un intento con una credencial desconocida también se registra.
        builder.HasOne<Credential>()
            .WithMany()
            .HasForeignKey(accessEvent => accessEvent.CredentialId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Employee>()
            .WithMany()
            .HasForeignKey(accessEvent => accessEvent.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<AccessPoint>()
            .WithMany()
            .HasForeignKey(accessEvent => accessEvent.AccessPointId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices pensados para las consultas del historial (RF-04) y la regla anti-passback.
        builder.HasIndex(accessEvent => new { accessEvent.EmployeeId, accessEvent.OccurredAt });
        builder.HasIndex(accessEvent => new { accessEvent.AccessPointId, accessEvent.OccurredAt });
        builder.HasIndex(accessEvent => new { accessEvent.Result, accessEvent.OccurredAt });
        builder.HasIndex(accessEvent => accessEvent.OccurredAt);
    }
}
