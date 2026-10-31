using MediatR;
using Sgcae.Application.Abstractions;
using Sgcae.Application.Common;

namespace Sgcae.Application.Employees.Queries;

public sealed record GetEmployeeByIdQuery(Guid EmployeeId) : IQuery<EmployeeDetails>;

public sealed class GetEmployeeByIdHandler(IEmployeeReadService readService)
    : IRequestHandler<GetEmployeeByIdQuery, EmployeeDetails>
{
    public async Task<EmployeeDetails> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken) =>
        await readService.GetByIdAsync(request.EmployeeId, cancellationToken)
        ?? throw new NotFoundException("Empleado", request.EmployeeId);
}
