using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Sgcae.Domain.Common;

/// <summary>
/// Correo electrónico normalizado en minúsculas.
/// </summary>
public sealed record EmailAddress
{
    public const int MaxLength = 150;

    private static readonly Regex Pattern = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    private EmailAddress(string value) => Value = value;

    public string Value { get; }

    public static EmailAddress Create(string? value)
    {
        var normalized = Guard.Required(value, "correo electrónico", MaxLength).ToLowerInvariant();
        if (!Pattern.IsMatch(normalized))
        {
            throw new DomainException($"'{value}' no es un correo electrónico válido.", "invalid_email");
        }

        return new EmailAddress(normalized);
    }

    public static bool TryCreate(string? value, [NotNullWhen(true)] out EmailAddress? email)
    {
        try
        {
            email = Create(value);
            return true;
        }
        catch (DomainException)
        {
            email = null;
            return false;
        }
    }

    public override string ToString() => Value;
}
