using FluentValidation;
using MediatR;
using Sgcae.Application.Abstractions;
using Sgcae.Application.Common;
using Sgcae.Domain.Common;
using Sgcae.Domain.Employees;

namespace Sgcae.Application.Employees.Commands;

public sealed record UpdateEmployeeCommand(
    Guid EmployeeId,
    string FirstNames,
    string LastNames,
    string Email,
    string JobTitle,
    string Department) : ICommand;

public sealed class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty();
        RuleFor(x => x.FirstNames).NotEmpty().MaximumLength(PersonName.MaxLength);
        RuleFor(x => x.LastNames).NotEmpty().MaximumLength(PersonName.MaxLength);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(EmailAddress.MaxLength);
        RuleFor(x => x.JobTitle).NotEmpty().MaximumLength(Employee.JobTitleMaxLength);
        RuleFor(x => x.Department).NotEmpty().MaximumLength(Employee.DepartmentMaxLength);
    }
}

public sealed class UpdateEmployeeHandler(
    IEmployeeRepository employees,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateEmployeeCommand>
{
    public async Task Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await employees.GetByIdAsync(request.EmployeeId, cancellationToken)
            ?? throw new NotFoundException("Empleado", request.EmployeeId);

        var email = EmailAddress.Create(request.Email);
        if (await employees.EmailInUseAsync(email, employee.Id, cancellationToken))
        {
            throw new ConflictException($"El correo {email} ya pertenece a otro empleado.");
        }

        employee.UpdateProfile(
            PersonName.Create(request.FirstNames, request.LastNames),
            email,
            request.JobTitle,
            request.Department);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
