using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Sgcae.Domain.Common;

namespace Sgcae.Domain.Credentials;

/// <summary>
/// Código impreso en la credencial, con formato <c>SGC-XXXX-XXXX</c>.
/// Omite los caracteres que se confunden al leerlos (I, O, 0 y 1).
/// </summary>
public sealed record CredentialCode
{
    public const int Length = 13;

    private const string Prefix = "SGC";
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    private static readonly Regex Pattern = new("^SGC-[A-HJ-NP-Z2-9]{4}-[A-HJ-NP-Z2-9]{4}$", RegexOptions.Compiled);

    private CredentialCode(string value) => Value = value;

    public string Value { get; }

    /// <summary>Genera un código aleatorio con un generador criptográficamente seguro.</summary>
    public static CredentialCode Generate()
    {
        var symbols = new char[8];
        for (var i = 0; i < symbols.Length; i++)
        {
            symbols[i] = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];
        }

        return new CredentialCode($"{Prefix}-{new string(symbols, 0, 4)}-{new string(symbols, 4, 4)}");
    }

    public static CredentialCode Create(string? value)
    {
        var normalized = Guard.Required(value, "código de la credencial", Length).ToUpperInvariant();
        if (!Pattern.IsMatch(normalized))
        {
            throw new DomainException(
                $"'{value}' no es un código de credencial válido (formato SGC-XXXX-XXXX).",
                "invalid_credential_code");
        }

        return new CredentialCode(normalized);
    }

    public static bool TryCreate(string? value, [NotNullWhen(true)] out CredentialCode? code)
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
