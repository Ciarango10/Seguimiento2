using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sgcae.Application.AccessHistory;
using Sgcae.Application.Common;
using Sgcae.Domain.AccessControl;

namespace Sgcae.WebApi.Controllers;

/// <summary>RF-04. Consulta del historial de accesos.</summary>
[ApiController]
[Route("api/access-history")]
[Produces("application/json")]
public sealed class AccessHistoryController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Consulta los movimientos registrados, del más reciente al más antiguo, con paginación.
    /// </summary>
    /// <param name="employeeId">Solo los movimientos de este empleado.</param>
    /// <param name="from">Desde esta fecha y hora (ISO 8601, por ejemplo 2026-10-31T00:00:00-05:00).</param>
    /// <param name="to">Hasta esta fecha y hora.</param>
    /// <param name="accessPointId">Solo los movimientos de este punto de acceso.</param>
    /// <param name="result">Granted o Denied.</param>
    /// <param name="page">Número de página, desde 1.</param>
    /// <param name="pageSize">Elementos por página (máximo 100).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AccessHistoryItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<PagedResult<AccessHistoryItem>> Search(
        [FromQuery] Guid? employeeId,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to,
        [FromQuery] Guid? accessPointId,
        [FromQuery] AccessResult? result,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = PagingRules.DefaultPageSize) =>
        await sender.Send(
            new GetAccessHistoryQuery(employeeId, from, to, accessPointId, result, page, pageSize),
            cancellationToken);
}
