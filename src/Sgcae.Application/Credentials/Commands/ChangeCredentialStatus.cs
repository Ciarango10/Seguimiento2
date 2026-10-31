using FluentValidation;
using MediatR;
using Sgcae.Application.Abstractions;
using Sgcae.Application.Common;
using Sgcae.Domain.Credentials;

namespace Sgcae.Application.Credentials.Commands;

public sealed record BlockCredentialCommand(string Code, string Reason) : ICommand;

public sealed record UnblockCredentialCommand(string Code) : ICommand;

/// <summary>Revoca la credencial de forma definitiva.</summary>
public sealed record RevokeCredentialCommand(string Code, string Reason) : ICommand;

public sealed class BlockCredentialValidator : AbstractValidator<BlockCredentialCommand>
{
    public BlockCredentialValidator()
    {
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(Credential.ReasonMaxLength);
    }
}

public sealed class RevokeCredentialValidator : AbstractValidator<RevokeCredentialCommand>
{
    public RevokeCredentialValidator()
    {
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(Credential.ReasonMaxLength);
    }
}

public sealed class ChangeCredentialStatusHandler(
    ICredentialRepository credentials,
    IUnitOfWork unitOfWork,
    TimeProvider clock) :
    IRequestHandler<BlockCredentialCommand>,
    IRequestHandler<UnblockCredentialCommand>,
    IRequestHandler<RevokeCredentialCommand>
{
    public Task Handle(BlockCredentialCommand request, CancellationToken cancellationToken) =>
        ChangeAsync(request.Code, credential => credential.Block(request.Reason, clock.GetUtcNow()), cancellationToken);

    public Task Handle(UnblockCredentialCommand request, CancellationToken cancellationToken) =>
        ChangeAsync(request.Code, credential => credential.Unblock(clock.GetUtcNow()), cancellationToken);

    public Task Handle(RevokeCredentialCommand request, CancellationToken cancellationToken) =>
        ChangeAsync(request.Code, credential => credential.Revoke(request.Reason, clock.GetUtcNow()), cancellationToken);

    private async Task ChangeAsync(string rawCode, Action<Credential> change, CancellationToken cancellationToken)
    {
        var credential = CredentialCode.TryCreate(rawCode, out var code)
            ? await credentials.GetByCodeAsync(code, cancellationToken)
            : null;

        if (credential is null)
        {
            throw new NotFoundException("Credencial", rawCode);
        }

        change(credential);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
