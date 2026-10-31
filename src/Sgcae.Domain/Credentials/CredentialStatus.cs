namespace Sgcae.Domain.Credentials;

public enum CredentialStatus
{
    /// <summary>Vigente y utilizable.</summary>
    Active = 1,

    /// <summary>Vigente pero suspendida temporalmente; puede desbloquearse.</summary>
    Blocked = 2,

    /// <summary>Anulada de forma definitiva.</summary>
    Revoked = 3,
}
