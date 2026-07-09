namespace ControleGastos.Domain.Enums;

/// <summary>
/// Tipo de uma transação financeira.
/// </summary>
public enum TransactionType
{
    /// <summary>Saída de dinheiro (gasto).</summary>
    Despesa = 0,

    /// <summary>Entrada de dinheiro.</summary>
    Receita = 1
}
