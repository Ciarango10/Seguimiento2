using Microsoft.EntityFrameworkCore;
using Sgcae.Application.Employees;
using Sgcae.Domain.Common;
using Sgcae.Domain.Employees;

namespace Sgcae.Infrastructure.Persistence.Repositories;

internal sealed class EmployeeRepository(SgcaeDbContext context) : IEmployeeRepository
{
    private DbSet<Employee> Employees => context.Set<Employee>();

    public Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Employees.FirstOrDefaultAsync(employee => employee.Id == id, cancellationToken);

    public Task<bool> DocumentExistsAsync(DocumentNumber document, CancellationToken cancellationToken) =>
        Employees.AnyAsync(employee => employee.Document == document, cancellationToken);

    public Task<bool> EmailInUseAsync(EmailAddress email, Guid? excludingEmployeeId, CancellationToken cancellationToken) =>
        excludingEmployeeId is { } excludedId
            ? Employees.AnyAsync(employee => employee.Email == email && employee.Id != excludedId, cancellationToken)
            : Employees.AnyAsync(employee => employee.Email == email, cancellationToken);

    public void Add(Employee employee) => Employees.Add(employee);
}
