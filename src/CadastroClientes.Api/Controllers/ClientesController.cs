using CadastroClientes.Application.Commands;
using CadastroClientes.Application.DTOs;
using CadastroClientes.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CadastroClientes.Api.Controllers;

[ApiController]
[Route("api/v1/clientes")]
public sealed class ClientesController : ControllerBase
{
    private readonly ISender _sender;

    public ClientesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ClientDto>> Create([FromBody] CreateClientCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ClientDto>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetClientByIdQuery(id), cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
