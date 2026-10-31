namespace Sgcae.Domain.Common;

/// <summary>
/// Raíz de agregado: es la única puerta de entrada para modificar los datos del agregado
/// y la responsable de mantener sus invariantes.
/// </summary>
public abstract class AggregateRoot : Entity
{
    protected AggregateRoot()
    {
    }

    protected AggregateRoot(Guid id) : base(id)
    {
    }
}
