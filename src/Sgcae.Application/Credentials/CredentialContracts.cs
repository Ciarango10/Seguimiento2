using Sgcae.Application.Common;
using Sgcae.Application.Credentials.Queries;
using Sgcae.Domain.Credentials;

namespace Sgcae.Application.Credentials;

/// <summary>Repositorio del agregado <see cref="Credential"/> (lado de escritura).</summary>
public interface ICredentialRepository
{
    Task<Credential?> GetByCodeAsync(CredentialCode code, CancellationToken cancellationToken);

    Task<bool> CodeExistsAsync(CredentialCode code, CancellationToken cancellationToken);

    /// <summary>Indica si el empleado ya tiene una credencial vigente (activa o bloqueada).</summary>
    Task<bool> HasCurrentCredentialAsync(Guid employeeId, CancellationToken cancellationToken);

    void Add(Credential credential);
}

/// <summary>Consultas de solo lectura sobre las credenciales.</summary>
public interface ICredentialReadService
{
    Task<CredentialDetails?> GetByCodeAsync(CredentialCode code, CancellationToken cancellationToken);

    Task<PagedResult<CredentialDetails>> SearchAsync(SearchCredentialsQuery query, CancellationToken cancellationToken);
}

public sealed record CredentialDetails(
    Guid Id,
    string Code,
    Guid EmployeeId,
    string EmployeeName,
    CredentialStatus Status,
    bool IsUsable,
    DateTimeOffset IssuedAt,
    DateTimeOffset? StatusChangedAt,
    string? StatusReason);

public sealed record IssuedCredential(Guid CredentialId, string Code);
