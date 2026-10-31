using Sgcae.Domain.Common;

namespace Sgcae.Domain.Employees;

/// <summary>Nombres y apellidos de una persona.</summary>
public sealed record PersonName
{
    public const int MaxLength = 80;

    private PersonName(string firstNames, string lastNames)
    {
        FirstNames = firstNames;
        LastNames = lastNames;
    }

    public string FirstNames { get; private set; }

    public string LastNames { get; private set; }

    public string FullName => $"{FirstNames} {LastNames}";

    public static PersonName Create(string? firstNames, string? lastNames) => new(
        Guard.Required(firstNames, "nombres", MaxLength),
        Guard.Required(lastNames, "apellidos", MaxLength));

    public override string ToString() => FullName;
}
