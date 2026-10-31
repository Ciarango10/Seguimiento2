using MediatR;
using Sgcae.Application.Abstractions;
using Sgcae.Application.Common;
using Sgcae.Domain.Employees;

namespace Sgcae.Application.Employees.Commands;

/// <summary>Desactiva al empleado: a partir de ese momento se le niega cualquier acceso.</summary>
public sealed record DeactivateEmployeeCommand(Guid EmployeeId) : ICommand;

public sealed record ReactivateEmployeeCommand(Guid EmployeeId) : ICommand;

public sealed class ChangeEmploymentStatusHandler(
    IEmployeeRepository employees,
    IUnitOfWork unitOfWork,
    TimeProvider clock) :
    IRequestHandler<DeactivateEmployeeCommand>,
    IRequestHandler<ReactivateEmployeeCommand>
{
    public Task Handle(DeactivateEmployeeCommand request, CancellationToken cancellationToken) =>
        ChangeAsync(request.EmployeeId, employee => employee.Deactivate(clock.GetUtcNow()), cancellationToken);

    public Task Handle(ReactivateEmployeeCommand request, CancellationToken cancellationToken) =>
        ChangeAsync(request.EmployeeId, employee => employee.Reactivate(), cancellationToken);

    private async Task ChangeAsync(Guid id, Action<Employee> change, CancellationToken cancellationToken)
    {
        var employee = await employees.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Empleado", id);

        change(employee);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
