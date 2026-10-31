using Sgcae.Domain.Credentials;

namespace Sgcae.Domain.AccessControl;

/// <summary>
/// Servicio de dominio que decide si un movimiento se autoriza. La regla involucra varios
/// agregados (credencial, empleado y punto de acceso), por eso no vive en ninguno de ellos.
/// </summary>
public static class AccessGate
{
    /// <summary>
    /// Evalúa las condiciones en orden y devuelve el primer motivo de rechazo encontrado:
    /// credencial → empleado → punto de acceso → coherencia entrada/salida (anti-passback).
    /// </summary>
    public static AccessDecision Evaluate(AccessAttempt attempt)
    {
        ArgumentNullException.ThrowIfNull(attempt);

        if (attempt.Credential is null || attempt.Employee is null)
        {
            return AccessDecision.Deny(DenialReason.CredentialNotFound);
        }

        switch (attempt.Credential.Status)
        {
            case CredentialStatus.Revoked:
                return AccessDecision.Deny(DenialReason.CredentialRevoked);
            case CredentialStatus.Blocked:
                return AccessDecision.Deny(DenialReason.CredentialBlocked);
        }

        if (!attempt.Employee.IsActive)
        {
            return AccessDecision.Deny(DenialReason.EmployeeInactive);
        }

        if (attempt.AccessPoint is null)
        {
            return AccessDecision.Deny(DenialReason.AccessPointNotFound);
        }

        if (!attempt.AccessPoint.IsEnabled)
        {
            return AccessDecision.Deny(DenialReason.AccessPointDisabled);
        }

        var isInside = attempt.LastGrantedDirection == MovementDirection.Entry;

        return attempt.Direction switch
        {
            MovementDirection.Entry when isInside => AccessDecision.Deny(DenialReason.AlreadyInside),
            MovementDirection.Exit when !isInside => AccessDecision.Deny(DenialReason.NotInside),
            _ => AccessDecision.Grant(),
        };
    }
}
