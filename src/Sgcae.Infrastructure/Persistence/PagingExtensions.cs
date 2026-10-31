using Microsoft.EntityFrameworkCore;
using Sgcae.Application.Common;

namespace Sgcae.Infrastructure.Persistence;

internal static class PagingExtensions
{
    /// <summary>
    /// Cuenta el total, trae solo la página pedida y la convierte a DTOs en memoria.
    /// La consulta debe venir ordenada para que la paginación sea estable.
    /// </summary>
    public static async Task<PagedResult<TResult>> ToPagedResultAsync<TSource, TResult>(
        this IQueryable<TSource> query,
        IPagedQuery paging,
        Func<TSource, TResult> map,
        CancellationToken cancellationToken)
    {
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((paging.Page - 1) * paging.PageSize)
            .Take(paging.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TResult>(items.Select(map).ToList(), paging.Page, paging.PageSize, total);
    }
}
