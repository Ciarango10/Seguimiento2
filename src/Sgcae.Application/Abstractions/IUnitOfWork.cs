namespace Sgcae.Application.Abstractions;

/// <summary>
/// Confirma en una sola transacción los cambios hechos por los repositorios durante un comando.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
