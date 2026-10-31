using FluentValidation;
using MediatR;
using Sgcae.Application.Abstractions;
using Sgcae.Application.Common;
using Sgcae.Domain.Common;
using Sgcae.Domain.Employees;

namespace Sgcae.Application.Employees.Commands;

public sealed record RegisterEmployeeCommand(
    string DocumentNumber,
    string FirstNames,
    string LastNames,
    string Email,
    string JobTitle,
    string Department) : ICommand<Guid>;

public sealed class RegisterEmployeeValidator : AbstractValidator<RegisterEmployeeCommand>
{
    public RegisterEmployeeValidator()
    {
        RuleFor(x => x.DocumentNumber).NotEmpty().MaximumLength(Domain.Employees.DocumentNumber.MaxLength);
        RuleFor(x => x.FirstNames).NotEmpty().MaximumLength(PersonName.MaxLength);
        RuleFor(x => x.LastNames).NotEmpty().MaximumLength(PersonName.MaxLength);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(EmailAddress.MaxLength);
        RuleFor(x => x.JobTitle).NotEmpty().MaximumLength(Employee.JobTitleMaxLength);
        RuleFor(x => x.Department).NotEmpty().MaximumLength(Employee.DepartmentMaxLength);
    }
}

public sealed class RegisterEmployeeHandler(
    IEmployeeRepository employees,
    IUnitOfWork unitOfWork,
    TimeProvider clock) : IRequestHandler<RegisterEmployeeCommand, Guid>
{
    public async Task<Guid> Handle(RegisterEmployeeCommand request, CancellationToken cancellationToken)
    {
        var document = DocumentNumber.Create(request.DocumentNumber);
        var email = EmailAddress.Create(request.Email);

        if (await employees.DocumentExistsAsync(document, cancellationToken))
        {
            throw new ConflictException($"Ya existe un empleado con la identificación {document}.");
        }

        if (await employees.EmailInUseAsync(email, excludingEmployeeId: null, cancellationToken))
        {
            throw new ConflictException($"El correo {email} ya pertenece a otro empleado.");
        }

        var employee = Employee.Register(
            document,
            PersonName.Create(request.FirstNames, request.LastNames),
            email,
            request.JobTitle,
            request.Department,
            clock.GetUtcNow());

        employees.Add(employee);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return employee.Id;
    }
}
