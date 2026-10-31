namespace Sgcae.Domain.AccessControl;

public enum MovementDirection
{
    Entry = 1,
    Exit = 2,
}

public enum AccessResult
{
    Granted = 1,
    Denied = 2,
}

/// <summary>Motivo por el que se rechaza un movimiento.</summary>
public enum DenialReason
{
    CredentialNotFound = 1,
    CredentialBlocked = 2,
    CredentialRevoked = 3,
    EmployeeInactive = 4,
    AccessPointNotFound = 5,
    AccessPointDisabled = 6,

    /// <summary>Intenta entrar cuando su último movimiento autorizado ya fue una entrada.</summary>
    AlreadyInside = 7,

    /// <summary>Intenta salir sin una entrada autorizada previa.</summary>
    NotInside = 8,
}
