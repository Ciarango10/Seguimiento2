using Microsoft.EntityFrameworkCore;
using Sgcae.Application.AccessHistory;
using Sgcae.Application.Common;
using Sgcae.Domain.AccessControl;
using Sgcae.Domain.AccessPoints;
using Sgcae.Domain.Employees;

namespace Sgcae.Infrastructure.Persistence.ReadServices;

internal sealed class AccessHistoryReadService(SgcaeDbContext context) : IAccessHistoryReadService
{
    public Task<PagedResult<AccessHistoryItem>> SearchAsync(GetAccessHistoryQuery query, CancellationToken cancellationToken)
    {
        var events = context.Set<AccessEvent>().AsNoTracking();

        // Los filtros se aplican sobre la tabla de eventos antes de unir con las demás,
        // para que SQL Server aproveche los índices por empleado, punto, resultado y fecha.
        if (query.EmployeeId is { } employeeId)
        {
            events = events.Where(item => item.EmployeeId == employeeId);
        }

        if (query.AccessPointId is { } accessPointId)
        {
            events = events.Where(item => item.AccessPointId == accessPointId);
        }

        if (query.Result is { } result)
        {
            events = events.Where(item => item.Result == result);
        }

        if (query.From is { } from)
        {
            events = events.Where(item => item.OccurredAt >= from);
        }

        if (query.To is { } to)
        {
            events = events.Where(item => item.OccurredAt <= to);
        }

        var rows =
            from accessEvent in events
            join employee in context.Set<Employee>().AsNoTracking()
                on accessEvent.EmployeeId equals (Guid?)employee.Id into employees
            from employee in employees.DefaultIfEmpty()
            join accessPoint in context.Set<AccessPoint>().AsNoTracking()
                on accessEvent.AccessPointId equals (Guid?)accessPoint.Id into accessPoints
            from accessPoint in accessPoints.DefaultIfEmpty()
            orderby accessEvent.OccurredAt descending
            select new HistoryRow
            {
                Event = accessEvent,
                FirstNames = employee.Name.FirstNames,
                LastNames = employee.Name.LastNames,
                AccessPointName = accessPoint.Name,
            };

        return rows.ToPagedResultAsync(query, ToItem, cancellationToken);
    }

    private static AccessHistoryItem ToItem(HistoryRow row) => new(
        row.Event.Id,
        row.Event.OccurredAt,
        row.Event.Direction,
        row.Event.Result,
        row.Event.DenialReason,
        row.Event.PresentedCredentialCode,
        row.Event.EmployeeId,
        row.FirstNames is null ? null : $"{row.FirstNames} {row.LastNames}",
        row.Event.PresentedAccessPointCode,
        row.Event.AccessPointId,
        row.AccessPointName);

    private sealed class HistoryRow
    {
        public required AccessEvent Event { get; init; }

        public string? FirstNames { get; init; }

        public string? LastNames { get; init; }

        public string? AccessPointName { get; init; }
    }
}
