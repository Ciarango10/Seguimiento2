namespace Sgcae.Domain.Common;

/// <summary>
/// Se lanza cuando una operación incumple una regla de negocio o una invariante del dominio.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message, string code) : base(message)
    {
        Code = code;
    }

    /// <summary>Código estable de la regla incumplida, útil para los clientes de la API.</summary>
    public string Code { get; }
}
