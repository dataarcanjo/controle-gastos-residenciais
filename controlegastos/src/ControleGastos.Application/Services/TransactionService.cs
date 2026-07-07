using ControleGastos.Application.DTOs;
using ControleGastos.Application.Interfaces;
using ControleGastos.Domain.Entities;
using ControleGastos.Domain.Enums;
using ControleGastos.Domain.Exceptions;
using ControleGastos.Domain.Interfaces;

namespace ControleGastos.Application.Services;

/// <summary>
/// Implementação das regras de negócio de cadastro de transações.
/// </summary>
public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IPersonRepository _personRepository;

    public TransactionService(ITransactionRepository transactionRepository, IPersonRepository personRepository)
    {
        _transactionRepository = transactionRepository;
        _personRepository = personRepository;
    }

    public async Task<TransactionResponse> CreateAsync(CreateTransactionRequest request, CancellationToken cancellationToken = default)
    {
        var type = ParseType(request.Type);

        // A pessoa precisa existir no cadastro de pessoas (regra explícita da especificação).
        var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken)
            ?? throw new EntityNotFoundException("Pessoa", request.PersonId);

        // Regra de negócio: pessoa menor de idade só pode ter despesas cadastradas.
        person.EnsureCanRegisterTransaction(type);

        // Transaction.Create valida descrição/valor e lança BusinessRuleViolationException se inválido.
        var transaction = Transaction.Create(request.Description, request.Value, type, request.PersonId);

        await _transactionRepository.AddAsync(transaction, cancellationToken);

        return ToResponse(transaction);
    }

    public async Task<IReadOnlyList<TransactionResponse>> GetAllAsync(Guid? personId = null, CancellationToken cancellationToken = default)
    {
        var transactions = personId.HasValue
            ? await _transactionRepository.GetByPersonIdAsync(personId.Value, cancellationToken)
            : await _transactionRepository.GetAllAsync(cancellationToken);

        return transactions.Select(ToResponse).ToList();
    }

    private static TransactionType ParseType(string type)
    {
        if (Enum.TryParse<TransactionType>(type, ignoreCase: true, out var parsed))
        {
            return parsed;
        }

        throw new BusinessRuleViolationException(
            $"Tipo de transação inválido: '{type}'. Valores aceitos: 'Despesa' ou 'Receita'.");
    }

    private static TransactionResponse ToResponse(Transaction transaction)
        => new(transaction.Id, transaction.Description, transaction.Value, transaction.Type.ToString(), transaction.PersonId);
}
