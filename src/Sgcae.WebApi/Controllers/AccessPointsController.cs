using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sgcae.Application.AccessPoints;
using Sgcae.Application.AccessPoints.Commands;
using Sgcae.Application.AccessPoints.Queries;
using Sgcae.Application.Common;
using Sgcae.Domain.AccessPoints;

namespace Sgcae.WebApi.Controllers;

/// <summary>RF-06. Administración de puntos de acceso.</summary>
[ApiController]
[Route("api/access-points")]
[Produces("application/json")]
public sealed class AccessPointsController(ISender sender) : ControllerBase
{
    /// <summary>Registra un punto de acceso físico. Queda habilitado al crearse.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(AccessPointDetails), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterAccessPointCommand command, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        var accessPoint = await sender.Send(new GetAccessPointByIdQuery(id), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, accessPoint);
    }

    /// <summary>Lista los puntos de acceso, con filtros opcionales y paginación.</summary>
    /// <param name="isEnabled">true para ver solo los habilitados; false para los deshabilitados.</param>
    /// <param name="kind">Tipo de punto: Reception, Parking, Warehouse, RestrictedArea, Office u Other.</param>
    /// <param name="name">Texto contenido en el nombre.</param>
    /// <param name="page">Número de página, desde 1.</param>
    /// <param name="pageSize">Elementos por página (máximo 100).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AccessPointDetails>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<PagedResult<AccessPointDetails>> Search(
        [FromQuery] bool? isEnabled,
        [FromQuery] AccessPointKind? kind,
        [FromQuery] string? name,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = PagingRules.DefaultPageSize) =>
        await sender.Send(new SearchAccessPointsQuery(isEnabled, kind, name, page, pageSize), cancellationToken);

    /// <summary>Consulta un punto de acceso por su identificador.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AccessPointDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<AccessPointDetails> GetById(Guid id, CancellationToken cancellationToken) =>
        await sender.Send(new GetAccessPointByIdQuery(id), cancellationToken);

    /// <summary>Habilita un punto de acceso para que vuelva a autorizar movimientos.</summary>
    [HttpPost("{id:guid}/enable")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Enable(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new EnableAccessPointCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>Deshabilita un punto de acceso: los movimientos que lleguen por él serán rechazados.</summary>
    [HttpPost("{id:guid}/disable")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Disable(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DisableAccessPointCommand(id), cancellationToken);
        return NoContent();
    }
}
