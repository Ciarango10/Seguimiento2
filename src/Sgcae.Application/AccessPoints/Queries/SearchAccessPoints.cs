using FluentValidation;
using MediatR;
using Sgcae.Application.Abstractions;
using Sgcae.Application.Common;
using Sgcae.Domain.AccessPoints;

namespace Sgcae.Application.AccessPoints.Queries;

/// <summary>Lista los puntos de acceso; permite filtrar por habilitados, tipo o nombre.</summary>
public sealed record SearchAccessPointsQuery(
    bool? IsEnabled,
    AccessPointKind? Kind,
    string? Name,
    int Page = 1,
    int PageSize = PagingRules.DefaultPageSize) : IQuery<PagedResult<AccessPointDetails>>, IPagedQuery;

public sealed class SearchAccessPointsValidator : AbstractValidator<SearchAccessPointsQuery>
{
    public SearchAccessPointsValidator()
    {
        this.AddPagingRules();
        RuleFor(x => x.Kind).IsInEnum().When(x => x.Kind.HasValue);
        RuleFor(x => x.Name).MaximumLength(AccessPoint.NameMaxLength);
    }
}

public sealed class SearchAccessPointsHandler(IAccessPointReadService readService)
    : IRequestHandler<SearchAccessPointsQuery, PagedResult<AccessPointDetails>>
{
    public Task<PagedResult<AccessPointDetails>> Handle(SearchAccessPointsQuery request, CancellationToken cancellationToken) =>
        readService.SearchAsync(request, cancellationToken);
}
