using Sgcae.Application.AccessPoints.Queries;
using Sgcae.Application.Common;
using Sgcae.Domain.AccessPoints;

namespace Sgcae.Application.AccessPoints;

/// <summary>Repositorio del agregado <see cref="AccessPoint"/> (lado de escritura).</summary>
public interface IAccessPointRepository
{
    Task<AccessPoint?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<AccessPoint?> GetByCodeAsync(AccessPointCode code, CancellationToken cancellationToken);

    Task<bool> CodeExistsAsync(AccessPointCode code, CancellationToken cancellationToken);

    void Add(AccessPoint accessPoint);
}

/// <summary>Consultas de solo lectura sobre los puntos de acceso.</summary>
public interface IAccessPointReadService
{
    Task<AccessPointDetails?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<PagedResult<AccessPointDetails>> SearchAsync(SearchAccessPointsQuery query, CancellationToken cancellationToken);
}

public sealed record AccessPointDetails(
    Guid Id,
    string Code,
    string Name,
    AccessPointKind Kind,
    string Location,
    bool IsEnabled,
    DateTimeOffset RegisteredAt);
