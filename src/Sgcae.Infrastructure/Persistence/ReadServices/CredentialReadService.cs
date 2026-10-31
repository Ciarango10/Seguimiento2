using Microsoft.EntityFrameworkCore;
using Sgcae.Application.Common;
using Sgcae.Application.Credentials;
using Sgcae.Application.Credentials.Queries;
using Sgcae.Domain.Credentials;
using Sgcae.Domain.Employees;

namespace Sgcae.Infrastructure.Persistence.ReadServices;

internal sealed class CredentialReadService(SgcaeDbContext context) : ICredentialReadService
{
    public async Task<CredentialDetails?> GetByCodeAsync(CredentialCode code, CancellationToken cancellationToken)
    {
        var row = await WithOwner()
            .FirstOrDefaultAsync(item => item.Credential.Code == code, cancellationToken);

        return row is null ? null : ToDetails(row);
    }

    public Task<PagedResult<CredentialDetails>> SearchAsync(SearchCredentialsQuery query, CancellationToken cancellationToken)
    {
        var rows = WithOwner();

        if (query.EmployeeId is { } employeeId)
        {
            rows = rows.Where(item => item.Credential.EmployeeId == employeeId);
        }

        if (query.Status is { } status)
        {
            rows = rows.Where(item => item.Credential.Status == status);
        }

        return rows
            .OrderByDescending(item => item.Credential.IssuedAt)
            .ToPagedResultAsync(query, ToDetails, cancellationToken);
    }

    private IQueryable<CredentialRow> WithOwner() =>
        from credential in context.Set<Credential>().AsNoTracking()
        join employee in context.Set<Employee>().AsNoTracking() on credential.EmployeeId equals employee.Id
        select new CredentialRow
        {
            Credential = credential,
            FirstNames = employee.Name.FirstNames,
            LastNames = employee.Name.LastNames,
        };

    private static CredentialDetails ToDetails(CredentialRow row) => new(
        row.Credential.Id,
        row.Credential.Code.Value,
        row.Credential.EmployeeId,
        $"{row.FirstNames} {row.LastNames}",
        row.Credential.Status,
        row.Credential.IsUsable,
        row.Credential.IssuedAt,
        row.Credential.StatusChangedAt,
        row.Credential.StatusReason);

    // Se proyecta con inicializador (no con constructor) para que EF pueda seguir
    // componiendo filtros y orden sobre la consulta.
    private sealed class CredentialRow
    {
        public required Credential Credential { get; init; }

        public required string FirstNames { get; init; }

        public required string LastNames { get; init; }
    }
}
