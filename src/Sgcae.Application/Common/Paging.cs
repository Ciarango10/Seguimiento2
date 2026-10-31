using FluentValidation;

namespace Sgcae.Application.Common;

/// <summary>Consulta que devuelve los resultados por páginas.</summary>
public interface IPagedQuery
{
    int Page { get; }

    int PageSize { get; }
}

/// <summary>Una página de resultados junto con los datos para navegar entre páginas.</summary>
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => TotalCount == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPreviousPage => Page > 1;

    public bool HasNextPage => Page < TotalPages;
}

public static class PagingRules
{
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    /// <summary>Agrega a un validador las reglas comunes de paginación.</summary>
    public static void AddPagingRules<T>(this AbstractValidator<T> validator) where T : IPagedQuery
    {
        validator.RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);
        validator.RuleFor(x => x.PageSize)
            .InclusiveBetween(1, MaxPageSize);
    }
}
