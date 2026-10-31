using Sgcae.Domain.AccessControl;

namespace Sgcae.Application.AccessControl;

/// <summary>Repositorio de los registros de acceso (lado de escritura).</summary>
public interface IAccessEventRepository
{
    /// <summary>Sentido del último movimiento autorizado del empleado, o null si nunca ha pasado.</summary>
    Task<MovementDirection?> GetLastGrantedDirectionAsync(Guid employeeId, CancellationToken cancellationToken);

    void Add(AccessEvent accessEvent);
}

public sealed record AccessEventResult(
    Guid EventId,
    AccessResult Result,
    DenialReason? DenialReason,
    string Message,
    MovementDirection Direction,
    Guid? EmployeeId,
    string? EmployeeName,
    string AccessPointCode,
    DateTimeOffset OccurredAt);

public static class AccessMessages
{
    public static string Describe(AccessDecision decision) => decision.Reason switch
    {
        null => "Acceso autorizado.",
        DenialReason.CredentialNotFound => "La credencial no está registrada.",
        DenialReason.CredentialBlocked => "La credencial está bloqueada.",
        DenialReason.CredentialRevoked => "La credencial fue revocada.",
        DenialReason.EmployeeInactive => "El empleado está inactivo.",
        DenialReason.AccessPointNotFound => "El punto de acceso no está registrado.",
        DenialReason.AccessPointDisabled => "El punto de acceso está deshabilitado.",
        DenialReason.AlreadyInside => "El empleado ya registró una entrada sin salida.",
        DenialReason.NotInside => "El empleado no tiene una entrada registrada.",
        _ => "Acceso rechazado.",
    };
}
