namespace Sgcae.Domain.Common;

/// <summary>
/// Entidad con identidad propia: dos entidades son iguales si son del mismo tipo y tienen el mismo Id.
/// </summary>
public abstract class Entity
{
    protected Entity()
    {
    }

    protected Entity(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("El identificador de la entidad no puede ser vacío.", "empty_id");
        }

        Id = id;
    }

    public Guid Id { get; protected set; }

    public override bool Equals(object? obj) =>
        obj is Entity other && other.GetType() == GetType() && other.Id == Id;

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
}
