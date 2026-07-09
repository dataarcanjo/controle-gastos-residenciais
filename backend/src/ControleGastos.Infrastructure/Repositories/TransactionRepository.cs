using ControleGastos.Domain.Entities;
using ControleGastos.Domain.Enums;
using ControleGastos.Domain.Interfaces;
using ControleGastos.Infrastructure.Persistence;

namespace ControleGastos.Infrastructure.Repositories;

public sealed class TransactionRepository : ITransactionRepository
{
    private readonly JsonDataStore _store;

    public TransactionRepository(JsonDataStore store)
    {
        _store = store;
    }

    public Task<IReadOnlyList<Transaction>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _store.ReadAsync<IReadOnlyList<Transaction>>(data =>
            data.Transactions.Select(ToDomain).ToList(), cancellationToken);
    }

    public Task<IReadOnlyList<Transaction>> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        return _store.ReadAsync<IReadOnlyList<Transaction>>(data =>
            data.Transactions.Where(t => t.PersonId == personId).Select(ToDomain).ToList(), cancellationToken);
    }

    public Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        return _store.WriteAsync(data =>
        {
            data.Transactions.Add(new TransactionRecord
            {
                Id = transaction.Id,
                Description = transaction.Description,
                Value = transaction.Value,
                Type = transaction.Type.ToString(),
                PersonId = transaction.PersonId
            });
        }, cancellationToken);
    }

    public Task DeleteByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        return _store.WriteAsync(data =>
        {
            data.Transactions.RemoveAll(t => t.PersonId == personId);
        }, cancellationToken);
    }

    private static Transaction ToDomain(TransactionRecord record)
        => Transaction.Restore(
            record.Id,
            record.Description,
            record.Value,
            Enum.Parse<TransactionType>(record.Type),
            record.PersonId);
}
