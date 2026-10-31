using Microsoft.EntityFrameworkCore;
using Sgcae.Application.Common;
using Sgcae.Application.Employees;
using Sgcae.Application.Employees.Queries;
using Sgcae.Domain.Employees;

namespace Sgcae.Infrastructure.Persistence.ReadServices;

internal sealed class EmployeeReadService(SgcaeDbContext context) : IEmployeeReadService
{
    public async Task<EmployeeDetails?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var employee = await context.Set<Employee>()
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return employee is null ? null : ToDetails(employee);
    }

    public Task<PagedResult<EmployeeSummary>> SearchAsync(SearchEmployeesQuery query, CancellationToken cancellationToken)
    {
        var employees = context.Set<Employee>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            employees = DocumentNumber.TryCreate(term, out var document)
                ? employees.Where(item =>
                    item.Name.FirstNames.Contains(term) || item.Name.LastNames.Contains(term) || item.Document == document)
                : employees.Where(item =>
                    item.Name.FirstNames.Contains(term) || item.Name.LastNames.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(query.Department))
        {
            var department = query.Department.Trim();
            employees = employees.Where(item => item.Department.Contains(department));
        }

        if (query.Status is { } status)
        {
            employees = employees.Where(item => item.Status == status);
        }

        return employees
            .OrderBy(item => item.Name.LastNames)
            .ThenBy(item => item.Name.FirstNames)
            .ToPagedResultAsync(query, ToSummary, cancellationToken);
    }

    private static EmployeeDetails ToDetails(Employee employee) => new(
        employee.Id,
        employee.Document.Value,
        employee.Name.FirstNames,
        employee.Name.LastNames,
        employee.Name.FullName,
        employee.Email.Value,
        employee.JobTitle,
        employee.Department,
        employee.Status,
        employee.RegisteredAt,
        employee.DeactivatedAt);

    private static EmployeeSummary ToSummary(Employee employee) => new(
        employee.Id,
        employee.Document.Value,
        employee.Name.FullName,
        employee.Email.Value,
        employee.JobTitle,
        employee.Department,
        employee.Status);
}
