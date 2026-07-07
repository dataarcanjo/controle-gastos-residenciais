using ControleGastos.Application.DTOs;

namespace ControleGastos.Application.Interfaces;

public interface IPersonService
{
    Task<PersonResponse> CreateAsync(CreatePersonRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PersonResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove a pessoa e, em cascata, todas as suas transações associadas.
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
