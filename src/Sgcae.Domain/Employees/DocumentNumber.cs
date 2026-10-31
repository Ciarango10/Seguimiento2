using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Sgcae.Domain.Common;

namespace Sgcae.Domain.Employees;

/// <summary>
/// Número de identificación del empleado (cédula, pasaporte, etc.). Es único en la organización.
/// </summary>
public sealed record DocumentNumber
{
    public const int MaxLength = 20;

    private static readonly Regex Pattern = new("^[A-Z0-9-]{5,20}$", RegexOptions.Compiled);

    private DocumentNumber(string value) => Value = value;

    public string Value { get; }

    public static DocumentNumber Create(string? value)
    {
        var normalized = Guard.Required(value, "número de identificación", MaxLength).ToUpperInvariant();
        if (!Pattern.IsMatch(normalized))
        {
            throw new DomainException(
                "El número de identificación solo admite letras, dígitos y guiones, entre 5 y 20 caracteres.",
                "invalid_document_number");
        }

        return new DocumentNumber(normalized);
    }

    public static bool TryCreate(string? value, [NotNullWhen(true)] out DocumentNumber? document)
    {
        try
        {
            document = Create(value);
            return true;
        }
        catch (DomainException)
        {
            document = null;
            return false;
        }
    }

    public override string ToString() => Value;
}
