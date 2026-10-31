using MediatR;
using Sgcae.Application.Abstractions;
using Sgcae.Application.Common;

namespace Sgcae.Application.AccessPoints.Queries;

public sealed record GetAccessPointByIdQuery(Guid AccessPointId) : IQuery<AccessPointDetails>;

public sealed class GetAccessPointByIdHandler(IAccessPointReadService readService)
    : IRequestHandler<GetAccessPointByIdQuery, AccessPointDetails>
{
    public async Task<AccessPointDetails> Handle(GetAccessPointByIdQuery request, CancellationToken cancellationToken) =>
        await readService.GetByIdAsync(request.AccessPointId, cancellationToken)
        ?? throw new NotFoundException("Punto de acceso", request.AccessPointId);
}
