using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sgcae.Domain.Common;
using Sgcae.Domain.Employees;

namespace Sgcae.Infrastructure.Persistence.Configurations;

internal sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");
        builder.HasKey(employee => employee.Id);
        builder.Property(employee => employee.Id).ValueGeneratedNever();

        builder.Property(employee => employee.Document)
            .HasColumnName("DocumentNumber")
            .HasConversion(document => document.Value, value => DocumentNumber.Create(value))
            .HasMaxLength(DocumentNumber.MaxLength)
            .IsRequired();
        builder.HasIndex(employee => employee.Document).IsUnique();

        builder.Property(employee => employee.Email)
            .HasConversion(email => email.Value, value => EmailAddress.Create(value))
            .HasMaxLength(EmailAddress.MaxLength)
            .IsRequired();
        builder.HasIndex(employee => employee.Email).IsUnique();

        // El nombre se guarda en dos columnas de la misma tabla.
        builder.OwnsOne(employee => employee.Name, name =>
        {
            name.Property(n => n.FirstNames).HasColumnName("FirstNames").HasMaxLength(PersonName.MaxLength).IsRequired();
            name.Property(n => n.LastNames).HasColumnName("LastNames").HasMaxLength(PersonName.MaxLength).IsRequired();
        });
        builder.Navigation(employee => employee.Name).IsRequired();

        builder.Property(employee => employee.JobTitle).HasMaxLength(Employee.JobTitleMaxLength).IsRequired();
        builder.Property(employee => employee.Department).HasMaxLength(Employee.DepartmentMaxLength).IsRequired();
        builder.Property(employee => employee.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(employee => employee.Department);
        builder.HasIndex(employee => employee.Status);
    }
}
