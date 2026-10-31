using Microsoft.EntityFrameworkCore;
using Sgcae.Application.AccessPoints;
using Sgcae.Domain.AccessPoints;

namespace Sgcae.Infrastructure.Persistence.Repositories;

internal sealed class AccessPointRepository(SgcaeDbContext context) : IAccessPointRepository
{
    private DbSet<AccessPoint> AccessPoints => context.Set<AccessPoint>();

    public Task<AccessPoint?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        AccessPoints.FirstOrDefaultAsync(accessPoint => accessPoint.Id == id, cancellationToken);

    public Task<AccessPoint?> GetByCodeAsync(AccessPointCode code, CancellationToken cancellationToken) =>
        AccessPoints.FirstOrDefaultAsync(accessPoint => accessPoint.Code == code, cancellationToken);

    public Task<bool> CodeExistsAsync(AccessPointCode code, CancellationToken cancellationToken) =>
        AccessPoints.AnyAsync(accessPoint => accessPoint.Code == code, cancellationToken);

    public void Add(AccessPoint accessPoint) => AccessPoints.Add(accessPoint);
}
