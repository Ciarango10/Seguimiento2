using Microsoft.EntityFrameworkCore;
using Sgcae.Application.AccessControl;
using Sgcae.Domain.AccessControl;

namespace Sgcae.Infrastructure.Persistence.Repositories;

internal sealed class AccessEventRepository(SgcaeDbContext context) : IAccessEventRepository
{
    private DbSet<AccessEvent> AccessEvents => context.Set<AccessEvent>();

    public Task<MovementDirection?> GetLastGrantedDirectionAsync(Guid employeeId, CancellationToken cancellationToken) =>
        AccessEvents
            .AsNoTracking()
            .Where(accessEvent => accessEvent.EmployeeId == employeeId && accessEvent.Result == AccessResult.Granted)
            .OrderByDescending(accessEvent => accessEvent.OccurredAt)
            .Select(accessEvent => (MovementDirection?)accessEvent.Direction)
            .FirstOrDefaultAsync(cancellationToken);

    public void Add(AccessEvent accessEvent) => AccessEvents.Add(accessEvent);
}
