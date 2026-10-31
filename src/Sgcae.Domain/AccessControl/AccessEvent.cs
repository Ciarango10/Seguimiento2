using Sgcae.Domain.Common;

namespace Sgcae.Domain.AccessControl;

/// <summary>
/// Registro inmutable de un intento de entrada o salida, autorizado o rechazado.
/// Guarda los códigos tal como se presentaron, aunque no correspondan a nada registrado,
/// para que el historial también muestre los intentos con credenciales desconocidas.
/// </summary>
public sealed class AccessEvent : AggregateRoot
{
    public const int PresentedCredentialCodeMaxLength = 30;
    public const int PresentedAccessPointCodeMaxLength = 30;

    private AccessEvent()
    {
    }

    private AccessEvent(Guid id, AccessAttempt attempt, AccessDecision decision, DateTimeOffset occurredAt) : base(id)
    {
        PresentedCredentialCode = Normalize(attempt.PresentedCredentialCode, PresentedCredentialCodeMaxLength);
        PresentedAccessPointCode = Normalize(attempt.PresentedAccessPointCode, PresentedAccessPointCodeMaxLength);
        CredentialId = attempt.Credential?.Id;
        EmployeeId = attempt.Employee?.Id;
        AccessPointId = attempt.AccessPoint?.Id;
        Direction = attempt.Direction;
        Result = decision.Result;
        DenialReason = decision.Reason;
        OccurredAt = occurredAt;
    }

    public string PresentedCredentialCode { get; private set; } = string.Empty;

    public string PresentedAccessPointCode { get; private set; } = string.Empty;

    public Guid? CredentialId { get; private set; }

    public Guid? EmployeeId { get; private set; }

    public Guid? AccessPointId { get; private set; }

    public MovementDirection Direction { get; private set; }

    public AccessResult Result { get; private set; }

    public DenialReason? DenialReason { get; private set; }

    public DateTimeOffset OccurredAt { get; private set; }

    public static AccessEvent Record(AccessAttempt attempt, AccessDecision decision, DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(attempt);
        ArgumentNullException.ThrowIfNull(decision);

        if (!Enum.IsDefined(attempt.Direction))
        {
            throw new DomainException("El tipo de movimiento debe ser Entry o Exit.", "invalid_direction");
        }

        return new AccessEvent(Guid.NewGuid(), attempt, decision, now);
    }

    private static string Normalize(string? value, int maxLength)
    {
        var normalized = (value ?? string.Empty).Trim().ToUpperInvariant();
        return normalized.Length <= maxLength ? normalized : normalized[..maxLength];
    }
}
