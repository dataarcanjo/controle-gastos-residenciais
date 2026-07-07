using ControleGastos.Domain.Entities;

namespace ControleGastos.Domain.Interfaces;

/// <summary>
/// Contrato de persistência para a entidade Person.
/// A camada de domínio/aplicação não sabe (nem precisa saber) qual é o
/// mecanismo de armazenamento real (arquivo, SQLite, SQL Server, etc.),
/// o que permite trocar a implementação em ControleGastos.Infrastructure
/// sem impactar regras de negócio — é o ponto que torna a solução escalável.
/// </summary>
public interface IPersonRepository
{
    Task<Person?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Person>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Person person, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove a pessoa. A remoção em cascata das transações associadas
    /// é responsabilidade da camada de aplicação (orquestração entre repositórios),
    /// mantendo cada repositório responsável por uma única entidade.
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
