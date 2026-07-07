using ControleGastos.Domain.Enums;
using ControleGastos.Domain.Exceptions;

namespace ControleGastos.Domain.Entities;

/// <summary>
/// Representa uma transação financeira (despesa ou receita) vinculada a uma pessoa.
/// </summary>
public class Transaction
{
    public Guid Id { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal Value { get; private set; }
    public TransactionType Type { get; private set; }
    public Guid PersonId { get; private set; }

    private Transaction()
    {
    }

    /// <summary>
    /// Cria uma nova transação já validando as regras de negócio básicas.
    /// A validação cruzada (pessoa existe / pessoa menor de idade) é responsabilidade
    /// da camada de aplicação, pois depende de consultar o repositório de pessoas.
    /// </summary>
    public static Transaction Create(string description, decimal value, TransactionType type, Guid personId)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new BusinessRuleViolationException("A descrição da transação é obrigatória.");
        }

        if (value <= 0)
        {
            throw new BusinessRuleViolationException("O valor da transação deve ser maior que zero.");
        }

        if (personId == Guid.Empty)
        {
            throw new BusinessRuleViolationException("A pessoa da transação é obrigatória.");
        }

        return new Transaction
        {
            Id = Guid.NewGuid(),
            Description = description.Trim(),
            Value = value,
            Type = type,
            PersonId = personId
        };
    }

    /// <summary>
    /// Reconstrói uma transação já existente a partir dos dados persistidos.
    /// Usado exclusivamente pela camada de infraestrutura ao carregar dados do armazenamento.
    /// </summary>
    public static Transaction Restore(Guid id, string description, decimal value, TransactionType type, Guid personId)
    {
        return new Transaction
        {
            Id = id,
            Description = description,
            Value = value,
            Type = type,
            PersonId = personId
        };
    }
}
