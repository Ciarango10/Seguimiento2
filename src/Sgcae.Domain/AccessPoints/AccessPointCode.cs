using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Sgcae.Domain.Common;

namespace Sgcae.Domain.AccessPoints;

/// <summary>
/// Código corto con el que los lectores físicos identifican el punto de acceso (por ejemplo, <c>REC-01</c>).
/// </summary>
public sealed record AccessPointCode
{
    public const int MaxLength = 20;

    private static readonly Regex Pattern = new("^[A-Z0-9]+(-[A-Z0-9]+)*$", RegexOptions.Compiled);

    private AccessPointCode(string value) => Value = value;

    public string Value { get; }

    public static AccessPointCode Create(string? value)
    {
        var normalized = Guard.Required(value, "código del punto de acceso", MaxLength).ToUpperInvariant();
        if (normalized.Length < 3 || !Pattern.IsMatch(normalized))
        {
            throw new DomainException(
                "El código del punto de acceso debe tener entre 3 y 20 letras o dígitos, separados opcionalmente por guiones.",
                "invalid_access_point_code");
        }

        return new AccessPointCode(normalized);
    }

    public static bool TryCreate(string? value, [NotNullWhen(true)] out AccessPointCode? code)
    {
        try
        {
            code = Create(value);
            return true;
        }
        catch (DomainException)
        {
            code = null;
            return false;
        }
    }

    public override string ToString() => Value;
}
