using MediatR;

namespace Sgcae.Application.Abstractions;

// CQRS: los comandos modifican el estado a través de los agregados;
// las consultas solo leen y devuelven DTOs.

/// <summary>Operación de escritura que no devuelve datos.</summary>
public interface ICommand : IRequest
{
}

/// <summary>Operación de escritura que devuelve un resultado (por ejemplo, el Id creado).</summary>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}

/// <summary>Operación de solo lectura.</summary>
public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
