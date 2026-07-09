using ControleGastos.Application.DTOs;

namespace ControleGastos.Application.Interfaces;

public interface ITransactionService
{
    Task<TransactionResponse> CreateAsync(CreateTransactionRequest request, CancellationToken cancellationToken = default);

    /// <summary>Lista todas as transações, ou apenas as de uma pessoa quando <paramref name="personId"/> é informado.</summary>
    Task<IReadOnlyList<TransactionResponse>> GetAllAsync(Guid? personId = null, CancellationToken cancellationToken = default);
}
