namespace ControleGastos.Application.DTOs;

/// <summary>Totais de receitas, despesas e saldo de uma pessoa específica.</summary>
public sealed record PersonTotalResponse(
    Guid PersonId,
    string Name,
    decimal TotalIncome,
    decimal TotalExpense,
    decimal Balance);

/// <summary>
/// Relatório completo de totais: os totais por pessoa e o total geral consolidado,
/// conforme exigido pela funcionalidade de "Consulta de totais".
/// </summary>
public sealed record TotalsReportResponse(
    IReadOnlyList<PersonTotalResponse> People,
    decimal GrandTotalIncome,
    decimal GrandTotalExpense,
    decimal GrandTotalBalance);
