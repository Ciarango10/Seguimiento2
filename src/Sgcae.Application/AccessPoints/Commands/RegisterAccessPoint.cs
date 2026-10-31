using FluentValidation;
using MediatR;
using Sgcae.Application.Abstractions;
using Sgcae.Application.Common;
using Sgcae.Domain.AccessPoints;

namespace Sgcae.Application.AccessPoints.Commands;

public sealed record RegisterAccessPointCommand(
    string Code,
    string Name,
    AccessPointKind Kind,
    string Location) : ICommand<Guid>;

public sealed class RegisterAccessPointValidator : AbstractValidator<RegisterAccessPointCommand>
{
    public RegisterAccessPointValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(AccessPointCode.MaxLength);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(AccessPoint.NameMaxLength);
        RuleFor(x => x.Kind).IsInEnum();
        RuleFor(x => x.Location).NotEmpty().MaximumLength(AccessPoint.LocationMaxLength);
    }
}

public sealed class RegisterAccessPointHandler(
    IAccessPointRepository accessPoints,
    IUnitOfWork unitOfWork,
    TimeProvider clock) : IRequestHandler<RegisterAccessPointCommand, Guid>
{
    public async Task<Guid> Handle(RegisterAccessPointCommand request, CancellationToken cancellationToken)
    {
        var code = AccessPointCode.Create(request.Code);
        if (await accessPoints.CodeExistsAsync(code, cancellationToken))
        {
            throw new ConflictException($"Ya existe un punto de acceso con el código {code}.");
        }

        var accessPoint = AccessPoint.Register(code, request.Name, request.Kind, request.Location, clock.GetUtcNow());

        accessPoints.Add(accessPoint);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return accessPoint.Id;
    }
}
