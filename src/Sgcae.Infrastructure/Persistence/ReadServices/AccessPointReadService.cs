using Microsoft.EntityFrameworkCore;
using Sgcae.Application.AccessPoints;
using Sgcae.Application.AccessPoints.Queries;
using Sgcae.Application.Common;
using Sgcae.Domain.AccessPoints;

namespace Sgcae.Infrastructure.Persistence.ReadServices;

internal sealed class AccessPointReadService(SgcaeDbContext context) : IAccessPointReadService
{
    public async Task<AccessPointDetails?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var accessPoint = await context.Set<AccessPoint>()
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return accessPoint is null ? null : ToDetails(accessPoint);
    }

    public Task<PagedResult<AccessPointDetails>> SearchAsync(SearchAccessPointsQuery query, CancellationToken cancellationToken)
    {
        var accessPoints = context.Set<AccessPoint>().AsNoTracking();

        if (query.IsEnabled is { } isEnabled)
        {
            accessPoints = accessPoints.Where(item => item.IsEnabled == isEnabled);
        }

        if (query.Kind is { } kind)
        {
            accessPoints = accessPoints.Where(item => item.Kind == kind);
        }

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            var name = query.Name.Trim();
            accessPoints = accessPoints.Where(item => item.Name.Contains(name));
        }

        return accessPoints
            .OrderBy(item => item.Name)
            .ToPagedResultAsync(query, ToDetails, cancellationToken);
    }

    private static AccessPointDetails ToDetails(AccessPoint accessPoint) => new(
        accessPoint.Id,
        accessPoint.Code.Value,
        accessPoint.Name,
        accessPoint.Kind,
        accessPoint.Location,
        accessPoint.IsEnabled,
        accessPoint.RegisteredAt);
}
