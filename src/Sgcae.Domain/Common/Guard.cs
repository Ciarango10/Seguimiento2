namespace Sgcae.Domain.Common;

internal static class Guard
{
    public static string Required(string? value, string field, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"El campo {field} es obligatorio.", "required_field");
        }

        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
        {
            throw new DomainException($"El campo {field} no puede superar {maxLength} caracteres.", "field_too_long");
        }

        return trimmed;
    }

    public static TEnum Defined<TEnum>(TEnum value, string field) where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
        {
            throw new DomainException($"El valor '{value}' no es válido para {field}.", "invalid_option");
        }

        return value;
    }
}
