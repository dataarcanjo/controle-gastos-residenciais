using ControleGastos.Application.DTOs;

namespace ControleGastos.Application.Interfaces;

public interface ITotalsService
{
    /// <summary>
    /// Monta o relatório de totais: receitas, despesas e saldo por pessoa,
    /// além do total geral consolidado de todas as pessoas.
    /// </summary>
    Task<TotalsReportResponse> GetReportAsync(CancellationToken cancellationToken = default);
}
