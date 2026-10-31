using Microsoft.EntityFrameworkCore;
using Sgcae.Application.Credentials;
using Sgcae.Domain.Credentials;

namespace Sgcae.Infrastructure.Persistence.Repositories;

internal sealed class CredentialRepository(SgcaeDbContext context) : ICredentialRepository
{
    private DbSet<Credential> Credentials => context.Set<Credential>();

    public Task<Credential?> GetByCodeAsync(CredentialCode code, CancellationToken cancellationToken) =>
        Credentials.FirstOrDefaultAsync(credential => credential.Code == code, cancellationToken);

    public Task<bool> CodeExistsAsync(CredentialCode code, CancellationToken cancellationToken) =>
        Credentials.AnyAsync(credential => credential.Code == code, cancellationToken);

    public Task<bool> HasCurrentCredentialAsync(Guid employeeId, CancellationToken cancellationToken) =>
        Credentials.AnyAsync(
            credential => credential.EmployeeId == employeeId && credential.Status != CredentialStatus.Revoked,
            cancellationToken);

    public void Add(Credential credential) => Credentials.Add(credential);
}
