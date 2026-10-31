using FluentValidation;
using MediatR;
using Sgcae.Application.Abstractions;
using Sgcae.Application.Common;
using Sgcae.Domain.AccessControl;

namespace Sgcae.Application.AccessHistory;

/// <summary>
/// Consulta paginada del historial de movimientos, del más reciente al más antiguo.
/// Todos los filtros son opcionales y se combinan entre sí.
/// </summary>
public sealed record GetAccessHistoryQuery(
    Guid? EmployeeId,
    DateTimeOffset? From,
    DateTimeOffset? To,
    Guid? AccessPointId,
    AccessResult? Result,
    int Page = 1,
    int PageSize = PagingRules.DefaultPageSize) : IQuery<PagedResult<AccessHistoryItem>>, IPagedQuery;

public sealed record AccessHistoryItem(
    Guid Id,
    DateTimeOffset OccurredAt,
    MovementDirection Direction,
    AccessResult Result,
    DenialReason? DenialReason,
    string CredentialCode,
    Guid? EmployeeId,
    string? EmployeeName,
    string AccessPointCode,
    Guid? AccessPointId,
    string? AccessPointName);

/// <summary>Lectura del historial; se implementa en infraestructura sin pasar por los agregados.</summary>
public interface IAccessHistoryReadService
{
    Task<PagedResult<AccessHistoryItem>> SearchAsync(GetAccessHistoryQuery query, CancellationToken cancellationToken);
}

public sealed class GetAccessHistoryValidator : AbstractValidator<GetAccessHistoryQuery>
{
    public GetAccessHistoryValidator()
    {
        this.AddPagingRules();
        RuleFor(x => x.Result).IsInEnum().When(x => x.Result.HasValue);
        RuleFor(x => x.To)
            .GreaterThanOrEqualTo(x => x.From)
            .When(x => x.From.HasValue && x.To.HasValue)
            .WithMessage("La fecha final debe ser igual o posterior a la fecha inicial.");
    }
}

public sealed class GetAccessHistoryHandler(IAccessHistoryReadService readService)
    : IRequestHandler<GetAccessHistoryQuery, PagedResult<AccessHistoryItem>>
{
    public Task<PagedResult<AccessHistoryItem>> Handle(GetAccessHistoryQuery request, CancellationToken cancellationToken) =>
        readService.SearchAsync(request, cancellationToken);
}
