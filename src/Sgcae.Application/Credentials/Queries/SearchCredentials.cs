using FluentValidation;
using MediatR;
using Sgcae.Application.Abstractions;
using Sgcae.Application.Common;
using Sgcae.Domain.Credentials;

namespace Sgcae.Application.Credentials.Queries;

/// <summary>Lista credenciales por empleado y/o estado, de la más reciente a la más antigua.</summary>
public sealed record SearchCredentialsQuery(
    Guid? EmployeeId,
    CredentialStatus? Status,
    int Page = 1,
    int PageSize = PagingRules.DefaultPageSize) : IQuery<PagedResult<CredentialDetails>>, IPagedQuery;

public sealed class SearchCredentialsValidator : AbstractValidator<SearchCredentialsQuery>
{
    public SearchCredentialsValidator()
    {
        this.AddPagingRules();
        RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
    }
}

public sealed class SearchCredentialsHandler(ICredentialReadService readService)
    : IRequestHandler<SearchCredentialsQuery, PagedResult<CredentialDetails>>
{
    public Task<PagedResult<CredentialDetails>> Handle(SearchCredentialsQuery request, CancellationToken cancellationToken) =>
        readService.SearchAsync(request, cancellationToken);
}
