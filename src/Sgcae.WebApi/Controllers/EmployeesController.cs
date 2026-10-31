using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sgcae.Application.Common;
using Sgcae.Application.Employees;
using Sgcae.Application.Employees.Commands;
using Sgcae.Application.Employees.Queries;
using Sgcae.Domain.Employees;
using Sgcae.WebApi.Requests;

namespace Sgcae.WebApi.Controllers;

/// <summary>RF-01. Gestión de empleados.</summary>
[ApiController]
[Route("api/employees")]
[Produces("application/json")]
public sealed class EmployeesController(ISender sender) : ControllerBase
{
    /// <summary>Registra un empleado. La identificación y el correo deben ser únicos.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(EmployeeDetails), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterEmployeeCommand command, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        var employee = await sender.Send(new GetEmployeeByIdQuery(id), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, employee);
    }

    /// <summary>Busca empleados con filtros opcionales y paginación.</summary>
    /// <param name="search">Texto en nombres o apellidos, o la identificación exacta.</param>
    /// <param name="department">Texto contenido en el área.</param>
    /// <param name="status">Active o Inactive.</param>
    /// <param name="page">Número de página, desde 1.</param>
    /// <param name="pageSize">Elementos por página (máximo 100).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EmployeeSummary>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<PagedResult<EmployeeSummary>> Search(
        [FromQuery] string? search,
        [FromQuery] string? department,
        [FromQuery] EmploymentStatus? status,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = PagingRules.DefaultPageSize) =>
        await sender.Send(new SearchEmployeesQuery(search, department, status, page, pageSize), cancellationToken);

    /// <summary>Consulta un empleado por su identificador.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EmployeeDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<EmployeeDetails> GetById(Guid id, CancellationToken cancellationToken) =>
        await sender.Send(new GetEmployeeByIdQuery(id), cancellationToken);

    /// <summary>Actualiza los datos de un empleado activo. La identificación no se puede cambiar.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(EmployeeDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<EmployeeDetails> Update(Guid id, UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(
            new UpdateEmployeeCommand(id, request.FirstNames, request.LastNames, request.Email, request.JobTitle, request.Department),
            cancellationToken);
        return await sender.Send(new GetEmployeeByIdQuery(id), cancellationToken);
    }

    /// <summary>Desactiva el vínculo laboral del empleado; desde ese momento se le niega el acceso.</summary>
    [HttpPost("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeactivateEmployeeCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>Reactiva a un empleado inactivo.</summary>
    [HttpPost("{id:guid}/reactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Reactivate(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new ReactivateEmployeeCommand(id), cancellationToken);
        return NoContent();
    }
}
