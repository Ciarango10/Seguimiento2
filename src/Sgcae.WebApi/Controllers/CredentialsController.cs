using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sgcae.Application.Common;
using Sgcae.Application.Credentials;
using Sgcae.Application.Credentials.Commands;
using Sgcae.Application.Credentials.Queries;
using Sgcae.Domain.Credentials;
using Sgcae.WebApi.Requests;

namespace Sgcae.WebApi.Controllers;

/// <summary>RF-02. Gestión de credenciales.</summary>
[ApiController]
[Route("api/credentials")]
[Produces("application/json")]
public sealed class CredentialsController(ISender sender) : ControllerBase
{
    /// <summary>Asigna una credencial nueva a un empleado activo. El código lo genera el sistema.</summary>
    [HttpPost("/api/employees/{employeeId:guid}/credentials")]
    [ProducesResponseType(typeof(CredentialDetails), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Assign(Guid employeeId, CancellationToken cancellationToken)
    {
        var issued = await sender.Send(new AssignCredentialCommand(employeeId), cancellationToken);
        var credential = await sender.Send(new GetCredentialByCodeQuery(issued.Code), cancellationToken);
        return CreatedAtAction(nameof(GetByCode), new { code = issued.Code }, credential);
    }

    /// <summary>Lista credenciales por empleado y/o estado, con paginación.</summary>
    /// <param name="employeeId">Identificador del empleado.</param>
    /// <param name="status">Active, Blocked o Revoked.</param>
    /// <param name="page">Número de página, desde 1.</param>
    /// <param name="pageSize">Elementos por página (máximo 100).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CredentialDetails>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<PagedResult<CredentialDetails>> Search(
        [FromQuery] Guid? employeeId,
        [FromQuery] CredentialStatus? status,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = PagingRules.DefaultPageSize) =>
        await sender.Send(new SearchCredentialsQuery(employeeId, status, page, pageSize), cancellationToken);

    /// <summary>Consulta una credencial y su estado por el código.</summary>
    [HttpGet("{code}")]
    [ProducesResponseType(typeof(CredentialDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<CredentialDetails> GetByCode(string code, CancellationToken cancellationToken) =>
        await sender.Send(new GetCredentialByCodeQuery(code), cancellationToken);

    /// <summary>Bloquea temporalmente la credencial.</summary>
    [HttpPost("{code}/block")]
    [ProducesResponseType(typeof(CredentialDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<CredentialDetails> Block(string code, StatusReasonRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new BlockCredentialCommand(code, request.Reason), cancellationToken);
        return await sender.Send(new GetCredentialByCodeQuery(code), cancellationToken);
    }

    /// <summary>Desbloquea una credencial bloqueada.</summary>
    [HttpPost("{code}/unblock")]
    [ProducesResponseType(typeof(CredentialDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<CredentialDetails> Unblock(string code, CancellationToken cancellationToken)
    {
        await sender.Send(new UnblockCredentialCommand(code), cancellationToken);
        return await sender.Send(new GetCredentialByCodeQuery(code), cancellationToken);
    }

    /// <summary>Revoca la credencial de forma definitiva.</summary>
    [HttpPost("{code}/revoke")]
    [ProducesResponseType(typeof(CredentialDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<CredentialDetails> Revoke(string code, StatusReasonRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new RevokeCredentialCommand(code, request.Reason), cancellationToken);
        return await sender.Send(new GetCredentialByCodeQuery(code), cancellationToken);
    }
}
