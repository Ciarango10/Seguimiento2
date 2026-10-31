using Sgcae.Domain.Common;

namespace Sgcae.Domain.AccessPoints;

/// <summary>
/// Punto físico donde se controla el paso de los empleados: recepción, parqueadero, bodega, etc.
/// Solo los puntos habilitados autorizan movimientos.
/// </summary>
public sealed class AccessPoint : AggregateRoot
{
    public const int NameMaxLength = 100;
    public const int LocationMaxLength = 150;

    private AccessPoint()
    {
    }

    private AccessPoint(
        Guid id,
        AccessPointCode code,
        string name,
        AccessPointKind kind,
        string location,
        DateTimeOffset registeredAt) : base(id)
    {
        Code = code;
        Name = name;
        Kind = kind;
        Location = location;
        IsEnabled = true;
        RegisteredAt = registeredAt;
    }

    public AccessPointCode Code { get; private set; } = null!;

    public string Name { get; private set; } = string.Empty;

    public AccessPointKind Kind { get; private set; }

    public string Location { get; private set; } = string.Empty;

    public bool IsEnabled { get; private set; }

    public DateTimeOffset RegisteredAt { get; private set; }

    /// <summary>Registra un punto de acceso nuevo; queda habilitado desde el inicio.</summary>
    public static AccessPoint Register(
        AccessPointCode code,
        string? name,
        AccessPointKind kind,
        string? location,
        DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(code);

        return new AccessPoint(
            Guid.NewGuid(),
            code,
            Guard.Required(name, "nombre", NameMaxLength),
            Guard.Defined(kind, "tipo de punto de acceso"),
            Guard.Required(location, "ubicación", LocationMaxLength),
            now);
    }

    public void Enable()
    {
        if (IsEnabled)
        {
            throw new DomainException($"El punto de acceso {Code} ya está habilitado.", "access_point_already_enabled");
        }

        IsEnabled = true;
    }

    public void Disable()
    {
        if (!IsEnabled)
        {
            throw new DomainException($"El punto de acceso {Code} ya está deshabilitado.", "access_point_already_disabled");
        }

        IsEnabled = false;
    }
}
