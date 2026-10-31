using Sgcae.Application.Common;
using Sgcae.Application.Employees.Queries;
using Sgcae.Domain.Common;
using Sgcae.Domain.Employees;

namespace Sgcae.Application.Employees;

/// <summary>Repositorio del agregado <see cref="Employee"/> (lado de escritura).</summary>
public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> DocumentExistsAsync(DocumentNumber document, CancellationToken cancellationToken);

    Task<bool> EmailInUseAsync(EmailAddress email, Guid? excludingEmployeeId, CancellationToken cancellationToken);

    void Add(Employee employee);
}

/// <summary>Consultas de solo lectura sobre los empleados.</summary>
public interface IEmployeeReadService
{
    Task<EmployeeDetails?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<PagedResult<EmployeeSummary>> SearchAsync(SearchEmployeesQuery query, CancellationToken cancellationToken);
}

public sealed record EmployeeDetails(
    Guid Id,
    string DocumentNumber,
    string FirstNames,
    string LastNames,
    string FullName,
    string Email,
    string JobTitle,
    string Department,
    EmploymentStatus Status,
    DateTimeOffset RegisteredAt,
    DateTimeOffset? DeactivatedAt);

public sealed record EmployeeSummary(
    Guid Id,
    string DocumentNumber,
    string FullName,
    string Email,
    string JobTitle,
    string Department,
    EmploymentStatus Status);
