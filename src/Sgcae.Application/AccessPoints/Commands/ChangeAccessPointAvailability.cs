using MediatR;
using Sgcae.Application.Abstractions;
using Sgcae.Application.Common;
using Sgcae.Domain.AccessPoints;

namespace Sgcae.Application.AccessPoints.Commands;

public sealed record EnableAccessPointCommand(Guid AccessPointId) : ICommand;

public sealed record DisableAccessPointCommand(Guid AccessPointId) : ICommand;

public sealed class ChangeAccessPointAvailabilityHandler(
    IAccessPointRepository accessPoints,
    IUnitOfWork unitOfWork) :
    IRequestHandler<EnableAccessPointCommand>,
    IRequestHandler<DisableAccessPointCommand>
{
    public Task Handle(EnableAccessPointCommand request, CancellationToken cancellationToken) =>
        ChangeAsync(request.AccessPointId, accessPoint => accessPoint.Enable(), cancellationToken);

    public Task Handle(DisableAccessPointCommand request, CancellationToken cancellationToken) =>
        ChangeAsync(request.AccessPointId, accessPoint => accessPoint.Disable(), cancellationToken);

    private async Task ChangeAsync(Guid id, Action<AccessPoint> change, CancellationToken cancellationToken)
    {
        var accessPoint = await accessPoints.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Punto de acceso", id);

        change(accessPoint);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
