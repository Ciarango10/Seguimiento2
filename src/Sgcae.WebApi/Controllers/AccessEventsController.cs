using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sgcae.Application.AccessControl;
using Sgcae.Application.AccessControl.Commands;

namespace Sgcae.WebApi.Controllers;

/// <summary>RF-03. Registro de entradas y salidas.</summary>
[ApiController]
[Route("api/access-events")]
[Produces("application/json")]
public sealed class AccessEventsController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Procesa una solicitud de acceso desde un lector. El movimiento siempre queda registrado;
    /// el campo <c>result</c> indica si se autorizó (Granted) o se rechazó (Denied) y por qué.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AccessEventResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RequestAccessCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }
}
