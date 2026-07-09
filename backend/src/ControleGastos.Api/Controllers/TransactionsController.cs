using ControleGastos.Application.DTOs;
using ControleGastos.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControleGastos.Api.Controllers;

/// <summary>Cadastro de transações: criação e listagem.</summary>
[ApiController]
[Route("api/transactions")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    /// <summary>
    /// Cadastra uma nova transação. Retorna 400 se o tipo/valor for inválido ou se a pessoa
    /// for menor de idade e a transação for do tipo "Receita"; retorna 404 se a pessoa não existir.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionResponse>> Create(
        [FromBody] CreateTransactionRequest request, CancellationToken cancellationToken)
    {
        var created = await _transactionService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { }, created);
    }

    /// <summary>Lista todas as transações. Pode ser filtrada por pessoa através de ?personId=.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TransactionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TransactionResponse>>> GetAll(
        [FromQuery] Guid? personId, CancellationToken cancellationToken)
    {
        var transactions = await _transactionService.GetAllAsync(personId, cancellationToken);
        return Ok(transactions);
    }
}
