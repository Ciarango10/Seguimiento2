using MediatR;
using Sgcae.Application.Abstractions;
using Sgcae.Application.Common;
using Sgcae.Domain.Credentials;

namespace Sgcae.Application.Credentials.Queries;

public sealed record GetCredentialByCodeQuery(string Code) : IQuery<CredentialDetails>;

public sealed class GetCredentialByCodeHandler(ICredentialReadService readService)
    : IRequestHandler<GetCredentialByCodeQuery, CredentialDetails>
{
    public async Task<CredentialDetails> Handle(GetCredentialByCodeQuery request, CancellationToken cancellationToken)
    {
        var details = CredentialCode.TryCreate(request.Code, out var code)
            ? await readService.GetByCodeAsync(code, cancellationToken)
            : null;

        return details ?? throw new NotFoundException("Credencial", request.Code);
    }
}
