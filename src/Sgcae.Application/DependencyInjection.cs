using System.Globalization;
using System.Text.Json;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Sgcae.Application.Common;

namespace Sgcae.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // Mensajes en español que nombran el campo tal como aparece en el JSON (por ejemplo, 'documentNumber').
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("es");
        ValidatorOptions.Global.DisplayNameResolver = (_, member, _) =>
            member is null ? null : JsonNamingPolicy.CamelCase.ConvertName(member.Name);
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
