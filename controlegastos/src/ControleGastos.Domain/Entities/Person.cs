using ControleGastos.Domain.Enums;
using ControleGastos.Domain.Exceptions;

namespace ControleGastos.Domain.Entities;

/// <summary>
/// Representa uma pessoa do grupo familiar/residência que pode ter
/// transações financeiras associadas a ela.
/// </summary>
public class Person
{
    /// <summary>Idade mínima considerada de maioridade para fins deste sistema.</summary>
    public const int IdadeMaioridade = 18;

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Age { get; private set; }

    /// <summary>
    /// Indica se a pessoa é menor de idade (regra de negócio: menor de 18 anos).
    /// Pessoas menores de idade só podem ter despesas cadastradas.
    /// </summary>
    public bool IsMinor => Age < IdadeMaioridade;

    // Necessário para frameworks de serialização/ORMs que exigem construtor sem parâmetros.
    private Person()
    {
    }

    /// <summary>
    /// Cria uma nova pessoa já validando as regras de negócio (nome obrigatório, idade válida).
    /// O identificador é gerado automaticamente, conforme especificação.
    /// </summary>
    public static Person Create(string name, int age)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleViolationException("O nome da pessoa é obrigatório.");
        }

        if (age < 0 || age > 130)
        {
            throw new BusinessRuleViolationException("A idade informada é inválida.");
        }

        return new Person
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Age = age
        };
    }

    /// <summary>
    /// Reconstrói uma pessoa já existente a partir dos dados persistidos.
    /// Usado exclusivamente pela camada de infraestrutura ao carregar dados do armazenamento
    /// (diferente de <see cref="Create"/>, não gera um novo identificador).
    /// </summary>
    public static Person Restore(Guid id, string name, int age)
    {
        return new Person
        {
            Id = id,
            Name = name,
            Age = age
        };
    }

    /// <summary>
    /// Regra de negócio central: pessoas menores de idade só podem registrar despesas.
    /// Lançada a partir do serviço de transações antes de persistir uma nova transação.
    /// </summary>
    public void EnsureCanRegisterTransaction(TransactionType type)
    {
        if (IsMinor && type == TransactionType.Receita)
        {
            throw new BusinessRuleViolationException(
                $"A pessoa '{Name}' é menor de idade ({Age} anos) e por isso só pode ter despesas cadastradas, não receitas.");
        }
    }
}
