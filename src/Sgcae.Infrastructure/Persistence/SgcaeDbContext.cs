using Microsoft.EntityFrameworkCore;
using Sgcae.Application.Abstractions;

namespace Sgcae.Infrastructure.Persistence;

/// <summary>
/// Contexto de EF Core sobre SQL Server. Cada agregado aporta su configuración
/// mediante una clase <see cref="IEntityTypeConfiguration{TEntity}"/>.
/// </summary>
public sealed class SgcaeDbContext(DbContextOptions<SgcaeDbContext> options) : DbContext(options), IUnitOfWork
{
    public const string Schema = "sgcae";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SgcaeDbContext).Assembly);
    }
}
