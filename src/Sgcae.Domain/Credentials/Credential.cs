using Sgcae.Domain.Common;
using Sgcae.Domain.Employees;

namespace Sgcae.Domain.Credentials;

/// <summary>
/// Credencial física asignada a un empleado.
/// <para>Estados: Active ⇄ Blocked → Revoked. Una credencial revocada ya no admite cambios.</para>
/// </summary>
public sealed class Credential : AggregateRoot
{
    public const int ReasonMaxLength = 200;

    private Credential()
    {
    }

    private Credential(Guid id, CredentialCode code, Guid employeeId, DateTimeOffset issuedAt) : base(id)
    {
        Code = code;
        EmployeeId = employeeId;
        Status = CredentialStatus.Active;
        IssuedAt = issuedAt;
    }

    public CredentialCode Code { get; private set; } = null!;

    public Guid EmployeeId { get; private set; }

    public CredentialStatus Status { get; private set; }

    public DateTimeOffset IssuedAt { get; private set; }

    public DateTimeOffset? StatusChangedAt { get; private set; }

    /// <summary>Motivo del último bloqueo o de la revocación.</summary>
    public string? StatusReason { get; private set; }

    /// <summary>Solo una credencial activa permite registrar movimientos.</summary>
    public bool IsUsable => Status == CredentialStatus.Active;

    /// <summary>Vigente = no revocada. Un empleado tiene como máximo una credencial vigente.</summary>
    public bool IsCurrent => Status != CredentialStatus.Revoked;

    /// <summary>Emite una credencial nueva. Solo los empleados activos pueden recibirla.</summary>
    public static Credential IssueFor(Employee employee, CredentialCode code, DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(employee);
        ArgumentNullException.ThrowIfNull(code);

        if (!employee.IsActive)
        {
            throw new DomainException(
                "Solo se pueden emitir credenciales para empleados activos.",
                "employee_inactive");
        }

        return new Credential(Guid.NewGuid(), code, employee.Id, now);
    }

    public void Block(string? reason, DateTimeOffset now)
    {
        EnsureNotRevoked();
        if (Status == CredentialStatus.Blocked)
        {
            throw new DomainException($"La credencial {Code} ya está bloqueada.", "credential_already_blocked");
        }

        ChangeStatus(CredentialStatus.Blocked, Guard.Required(reason, "motivo", ReasonMaxLength), now);
    }

    public void Unblock(DateTimeOffset now)
    {
        EnsureNotRevoked();
        if (Status != CredentialStatus.Blocked)
        {
            throw new DomainException($"La credencial {Code} no está bloqueada.", "credential_not_blocked");
        }

        ChangeStatus(CredentialStatus.Active, reason: null, now);
    }

    public void Revoke(string? reason, DateTimeOffset now)
    {
        EnsureNotRevoked();
        ChangeStatus(CredentialStatus.Revoked, Guard.Required(reason, "motivo", ReasonMaxLength), now);
    }

    private void EnsureNotRevoked()
    {
        if (Status == CredentialStatus.Revoked)
        {
            throw new DomainException(
                $"La credencial {Code} está revocada y no admite cambios.",
                "credential_revoked");
        }
    }

    private void ChangeStatus(CredentialStatus status, string? reason, DateTimeOffset now)
    {
        Status = status;
        StatusReason = reason;
        StatusChangedAt = now;
    }
}
