using Sgcae.Domain.Common;

namespace Sgcae.Domain.Employees;

/// <summary>
/// Empleado de la organización. Un empleado inactivo conserva su historial,
/// pero no puede modificarse ni recibir credenciales hasta que se reactive.
/// </summary>
public sealed class Employee : AggregateRoot
{
    public const int JobTitleMaxLength = 100;
    public const int DepartmentMaxLength = 100;

    private Employee()
    {
    }

    private Employee(
        Guid id,
        DocumentNumber document,
        PersonName name,
        EmailAddress email,
        string jobTitle,
        string department,
        DateTimeOffset registeredAt) : base(id)
    {
        Document = document;
        Name = name;
        Email = email;
        JobTitle = jobTitle;
        Department = department;
        Status = EmploymentStatus.Active;
        RegisteredAt = registeredAt;
    }

    public DocumentNumber Document { get; private set; } = null!;

    public PersonName Name { get; private set; } = null!;

    public EmailAddress Email { get; private set; } = null!;

    public string JobTitle { get; private set; } = string.Empty;

    /// <summary>Área organizacional a la que pertenece el empleado.</summary>
    public string Department { get; private set; } = string.Empty;

    public EmploymentStatus Status { get; private set; }

    public DateTimeOffset RegisteredAt { get; private set; }

    public DateTimeOffset? DeactivatedAt { get; private set; }

    public bool IsActive => Status == EmploymentStatus.Active;

    public static Employee Register(
        DocumentNumber document,
        PersonName name,
        EmailAddress email,
        string? jobTitle,
        string? department,
        DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(email);

        return new Employee(
            Guid.NewGuid(),
            document,
            name,
            email,
            Guard.Required(jobTitle, "cargo", JobTitleMaxLength),
            Guard.Required(department, "área", DepartmentMaxLength),
            now);
    }

    /// <summary>
    /// Actualiza los datos del empleado. La identificación no cambia: es la identidad de negocio.
    /// </summary>
    public void UpdateProfile(PersonName name, EmailAddress email, string? jobTitle, string? department)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(email);

        if (!IsActive)
        {
            throw new DomainException(
                "No se puede modificar un empleado inactivo; reactívelo primero.",
                "employee_inactive");
        }

        Name = name;
        Email = email;
        JobTitle = Guard.Required(jobTitle, "cargo", JobTitleMaxLength);
        Department = Guard.Required(department, "área", DepartmentMaxLength);
    }

    public void Deactivate(DateTimeOffset now)
    {
        if (!IsActive)
        {
            throw new DomainException("El empleado ya está inactivo.", "employee_already_inactive");
        }

        Status = EmploymentStatus.Inactive;
        DeactivatedAt = now;
    }

    public void Reactivate()
    {
        if (IsActive)
        {
            throw new DomainException("El empleado ya está activo.", "employee_already_active");
        }

        Status = EmploymentStatus.Active;
        DeactivatedAt = null;
    }
}
