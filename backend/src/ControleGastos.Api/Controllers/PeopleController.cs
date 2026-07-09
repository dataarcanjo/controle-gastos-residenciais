using ControleGastos.Application.DTOs;
using ControleGastos.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControleGastos.Api.Controllers;

/// <summary>Cadastro de pessoas: criação, deleção (com cascata de transações) e listagem.</summary>
[ApiController]
[Route("api/people")]
public class PeopleController : ControllerBase
{
    private readonly IPersonService _personService;

    public PeopleController(IPersonService personService)
    {
        _personService = personService;
    }

    /// <summary>Cadastra uma nova pessoa.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(PersonResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<PersonResponse>> Create(
        [FromBody] CreatePersonRequest request, CancellationToken cancellationToken)
    {
        var created = await _personService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { }, created);
    }

    /// <summary>Lista todas as pessoas cadastradas.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PersonResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PersonResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var people = await _personService.GetAllAsync(cancellationToken);
        return Ok(people);
    }

    /// <summary>
    /// Remove uma pessoa. Todas as transações associadas a ela são removidas em cascata.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _personService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
