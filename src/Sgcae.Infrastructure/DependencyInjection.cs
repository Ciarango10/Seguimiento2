using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sgcae.Application.Abstractions;
using Sgcae.Infrastructure.Persistence;

namespace Sgcae.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<SgcaeDbContext>(options => options.UseSqlServer(
            connectionString,
            sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", SgcaeDbContext.Schema)));

        // El mismo contexto que usan los repositorios confirma los cambios.
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<SgcaeDbContext>());

        // Registra por convención cada repositorio y servicio de lectura que implemente
        // un contrato definido en la capa de aplicación.
        var implementations = typeof(DependencyInjection).Assembly.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false } && !typeof(DbContext).IsAssignableFrom(type));

        foreach (var implementation in implementations)
        {
            var contracts = implementation.GetInterfaces()
                .Where(contract => contract.Namespace?.StartsWith("Sgcae.Application", StringComparison.Ordinal) == true);

            foreach (var contract in contracts)
            {
                services.AddScoped(contract, implementation);
            }
        }

        return services;
    }

    public static async Task ApplyMigrationsAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SgcaeDbContext>();
        await context.Database.MigrateAsync();
    }
}
