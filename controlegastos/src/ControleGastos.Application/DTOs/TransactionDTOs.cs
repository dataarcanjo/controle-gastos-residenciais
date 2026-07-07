namespace ControleGastos.Application.DTOs;

/// <summary>
/// Dados de entrada para o cadastro de uma transação.
/// Type é recebido como texto ("Despesa" ou "Receita") para manter a API
/// legível a quem a consome, e é convertido para o enum de domínio no serviço.
/// </summary>
public sealed record CreateTransactionRequest(string Description, decimal Value, string Type, Guid PersonId);

/// <summary>Representação de saída de uma transação.</summary>
public sealed record TransactionResponse(Guid Id, string Description, decimal Value, string Type, Guid PersonId);
