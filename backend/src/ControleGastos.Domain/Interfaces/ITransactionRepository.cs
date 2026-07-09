using ControleGastos.Domain.Entities;

namespace ControleGastos.Domain.Interfaces;

/// <summary>
/// Contrato de persistência para a entidade Transaction.
/// </summary>
public interface ITransactionRepository
{
    Task<IReadOnlyList<Transaction>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Transaction>> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default);

    Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default);

    /// <summary>Remove todas as transações de uma pessoa (usado na exclusão em cascata).</summary>
    Task DeleteByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default);
}
