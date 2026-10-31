using FluentValidation;
using MediatR;
using Sgcae.Application.Abstractions;
using Sgcae.Application.Common;
using Sgcae.Domain.Employees;

namespace Sgcae.Application.Employees.Queries;

/// <summary>
/// Busca empleados. <paramref name="Search"/> coincide con nombres, apellidos o la identificación exacta.
/// </summary>
public sealed record SearchEmployeesQuery(
    string? Search,
    string? Department,
    EmploymentStatus? Status,
    int Page = 1,
    int PageSize = PagingRules.DefaultPageSize) : IQuery<PagedResult<EmployeeSummary>>, IPagedQuery;

public sealed class SearchEmployeesValidator : AbstractValidator<SearchEmployeesQuery>
{
    public SearchEmployeesValidator()
    {
        this.AddPagingRules();
        RuleFor(x => x.Search).MaximumLength(100);
        RuleFor(x => x.Department).MaximumLength(Employee.DepartmentMaxLength);
        RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
    }
}

public sealed class SearchEmployeesHandler(IEmployeeReadService readService)
    : IRequestHandler<SearchEmployeesQuery, PagedResult<EmployeeSummary>>
{
    public Task<PagedResult<EmployeeSummary>> Handle(SearchEmployeesQuery request, CancellationToken cancellationToken) =>
        readService.SearchAsync(request, cancellationToken);
}
