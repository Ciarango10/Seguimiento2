using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sgcae.Domain.Credentials;
using Sgcae.Domain.Employees;

namespace Sgcae.Infrastructure.Persistence.Configurations;

internal sealed class CredentialConfiguration : IEntityTypeConfiguration<Credential>
{
    public void Configure(EntityTypeBuilder<Credential> builder)
    {
        builder.ToTable("Credentials");
        builder.HasKey(credential => credential.Id);
        builder.Property(credential => credential.Id).ValueGeneratedNever();

        builder.Property(credential => credential.Code)
            .HasConversion(code => code.Value, value => CredentialCode.Create(value))
            .HasMaxLength(CredentialCode.Length)
            .IsFixedLength()
            .IsRequired();
        builder.HasIndex(credential => credential.Code).IsUnique();

        builder.Property(credential => credential.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(credential => credential.StatusReason).HasMaxLength(Credential.ReasonMaxLength);

        builder.HasOne<Employee>()
            .WithMany()
            .HasForeignKey(credential => credential.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Respaldo en base de datos de la regla "una sola credencial vigente por empleado".
        builder.HasIndex(credential => credential.EmployeeId)
            .IsUnique()
            .HasFilter("[Status] <> 'Revoked'")
            .HasDatabaseName("UX_Credentials_EmployeeId_Current");
    }
}
