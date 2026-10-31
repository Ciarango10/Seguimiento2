using FluentValidation;
using MediatR;
using Sgcae.Application.Abstractions;
using Sgcae.Application.Common;
using Sgcae.Application.Employees;
using Sgcae.Domain.Credentials;

namespace Sgcae.Application.Credentials.Commands;

/// <summary>Asigna una credencial nueva con un código generado por el sistema.</summary>
public sealed record AssignCredentialCommand(Guid EmployeeId) : ICommand<IssuedCredential>;

public sealed class AssignCredentialValidator : AbstractValidator<AssignCredentialCommand>
{
    public AssignCredentialValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty();
    }
}

public sealed class AssignCredentialHandler(
    IEmployeeRepository employees,
    ICredentialRepository credentials,
    IUnitOfWork unitOfWork,
    TimeProvider clock) : IRequestHandler<AssignCredentialCommand, IssuedCredential>
{
    private const int MaxCodeAttempts = 5;

    public async Task<IssuedCredential> Handle(AssignCredentialCommand request, CancellationToken cancellationToken)
    {
        var employee = await employees.GetByIdAsync(request.EmployeeId, cancellationToken)
            ?? throw new NotFoundException("Empleado", request.EmployeeId);

        if (await credentials.HasCurrentCredentialAsync(employee.Id, cancellationToken))
        {
            throw new ConflictException(
                "El empleado ya tiene una credencial vigente; revóquela antes de asignarle una nueva.");
        }

        var code = await GenerateUniqueCodeAsync(cancellationToken);
        var credential = Credential.IssueFor(employee, code, clock.GetUtcNow());

        credentials.Add(credential);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new IssuedCredential(credential.Id, credential.Code.Value);
    }

    private async Task<CredentialCode> GenerateUniqueCodeAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < MaxCodeAttempts; attempt++)
        {
            var code = CredentialCode.Generate();
            if (!await credentials.CodeExistsAsync(code, cancellationToken))
            {
                return code;
            }
        }

        throw new ConflictException("No fue posible generar un código de credencial único; intente de nuevo.");
    }
}
