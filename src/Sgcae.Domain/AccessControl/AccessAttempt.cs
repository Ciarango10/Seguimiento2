using Sgcae.Domain.AccessPoints;
using Sgcae.Domain.Credentials;
using Sgcae.Domain.Employees;

namespace Sgcae.Domain.AccessControl;

/// <summary>
/// Lo que llega desde el lector (códigos tal como se presentaron y el sentido del movimiento)
/// junto con lo que se encontró en el sistema para evaluarlo.
/// </summary>
public sealed record AccessAttempt(
    string PresentedCredentialCode,
    string PresentedAccessPointCode,
    MovementDirection Direction,
    Credential? Credential,
    Employee? Employee,
    AccessPoint? AccessPoint,
    MovementDirection? LastGrantedDirection);

public sealed record AccessDecision(AccessResult Result, DenialReason? Reason)
{
    public static AccessDecision Grant() => new(AccessResult.Granted, null);

    public static AccessDecision Deny(DenialReason reason) => new(AccessResult.Denied, reason);

    public bool IsGranted => Result == AccessResult.Granted;
}
