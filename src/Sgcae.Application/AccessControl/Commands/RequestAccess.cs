using FluentValidation;
using MediatR;
using Sgcae.Application.Abstractions;
using Sgcae.Application.AccessPoints;
using Sgcae.Application.Credentials;
using Sgcae.Application.Employees;
using Sgcae.Domain.AccessControl;
using Sgcae.Domain.AccessPoints;
using Sgcae.Domain.Credentials;
using Sgcae.Domain.Employees;

namespace Sgcae.Application.AccessControl.Commands;

/// <summary>
/// Solicitud que envía un lector cuando alguien presenta su credencial.
/// Siempre deja un registro, se autorice o no.
/// </summary>
public sealed record RequestAccessCommand(
    string CredentialCode,
    string AccessPointCode,
    MovementDirection Direction) : ICommand<AccessEventResult>;

public sealed class RequestAccessValidator : AbstractValidator<RequestAccessCommand>
{
    public RequestAccessValidator()
    {
        RuleFor(x => x.CredentialCode).NotEmpty().MaximumLength(AccessEvent.PresentedCredentialCodeMaxLength);
        RuleFor(x => x.AccessPointCode).NotEmpty().MaximumLength(AccessEvent.PresentedAccessPointCodeMaxLength);
        RuleFor(x => x.Direction).IsInEnum();
    }
}

public sealed class RequestAccessHandler(
    ICredentialRepository credentials,
    IEmployeeRepository employees,
    IAccessPointRepository accessPoints,
    IAccessEventRepository accessEvents,
    IUnitOfWork unitOfWork,
    TimeProvider clock) : IRequestHandler<RequestAccessCommand, AccessEventResult>
{
    public async Task<AccessEventResult> Handle(RequestAccessCommand request, CancellationToken cancellationToken)
    {
        Credential? credential = null;
        Employee? employee = null;
        MovementDirection? lastGrantedDirection = null;

        if (CredentialCode.TryCreate(request.CredentialCode, out var credentialCode))
        {
            credential = await credentials.GetByCodeAsync(credentialCode, cancellationToken);
        }

        if (credential is not null)
        {
            employee = await employees.GetByIdAsync(credential.EmployeeId, cancellationToken);
            lastGrantedDirection = await accessEvents.GetLastGrantedDirectionAsync(credential.EmployeeId, cancellationToken);
        }

        var accessPoint = AccessPointCode.TryCreate(request.AccessPointCode, out var accessPointCode)
            ? await accessPoints.GetByCodeAsync(accessPointCode, cancellationToken)
            : null;

        var attempt = new AccessAttempt(
            request.CredentialCode,
            request.AccessPointCode,
            request.Direction,
            credential,
            employee,
            accessPoint,
            lastGrantedDirection);

        var decision = AccessGate.Evaluate(attempt);
        var accessEvent = AccessEvent.Record(attempt, decision, clock.GetUtcNow());

        accessEvents.Add(accessEvent);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AccessEventResult(
            accessEvent.Id,
            accessEvent.Result,
            accessEvent.DenialReason,
            AccessMessages.Describe(decision),
            accessEvent.Direction,
            employee?.Id,
            employee?.Name.FullName,
            accessEvent.PresentedAccessPointCode,
            accessEvent.OccurredAt);
    }
}
