namespace Sgcae.Application.Common;

/// <summary>El recurso solicitado no existe.</summary>
public sealed class NotFoundException(string resource, object key)
    : Exception($"{resource} '{key}' no existe.");

/// <summary>La operación choca con el estado actual de los datos (por ejemplo, un valor duplicado).</summary>
public sealed class ConflictException(string message) : Exception(message);
