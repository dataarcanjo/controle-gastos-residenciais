using ControleGastos.Application.DTOs;
using ControleGastos.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControleGastos.Api.Controllers;

/// <summary>Consulta de totais de receitas, despesas e saldo por pessoa e geral.</summary>
[ApiController]
[Route("api/totals")]
public class TotalsController : ControllerBase
{
    private readonly ITotalsService _totalsService;

    public TotalsController(ITotalsService totalsService)
    {
        _totalsService = totalsService;
    }

    /// <summary>
    /// Retorna o total de receitas, despesas e saldo de cada pessoa cadastrada,
    /// além do total geral consolidado de todas as pessoas.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(TotalsReportResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<TotalsReportResponse>> Get(CancellationToken cancellationToken)
    {
        var report = await _totalsService.GetReportAsync(cancellationToken);
        return Ok(report);
    }
}
