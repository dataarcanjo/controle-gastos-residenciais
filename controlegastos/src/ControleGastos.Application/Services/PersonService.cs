using ControleGastos.Application.DTOs;
using ControleGastos.Application.Interfaces;
using ControleGastos.Domain.Entities;
using ControleGastos.Domain.Exceptions;
using ControleGastos.Domain.Interfaces;

namespace ControleGastos.Application.Services;

/// <summary>
/// Implementação das regras de negócio de cadastro de pessoas.
/// </summary>
public class PersonService : IPersonService
{
    private readonly IPersonRepository _personRepository;
    private readonly ITransactionRepository _transactionRepository;

    public PersonService(IPersonRepository personRepository, ITransactionRepository transactionRepository)
    {
        _personRepository = personRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<PersonResponse> CreateAsync(CreatePersonRequest request, CancellationToken cancellationToken = default)
    {
        // Person.Create já valida nome/idade e lança BusinessRuleViolationException se inválido.
        var person = Person.Create(request.Name, request.Age);

        await _personRepository.AddAsync(person, cancellationToken);

        return ToResponse(person);
    }

    public async Task<IReadOnlyList<PersonResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var people = await _personRepository.GetAllAsync(cancellationToken);
        return people.Select(ToResponse).ToList();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var exists = await _personRepository.ExistsAsync(id, cancellationToken);
        if (!exists)
        {
            throw new EntityNotFoundException("Pessoa", id);
        }

        // Regra de negócio: ao deletar uma pessoa, todas as suas transações são apagadas.
        // A remoção das transações ocorre antes da remoção da pessoa para evitar
        // deixar transações "órfãs" caso algo falhe no meio do processo.
        await _transactionRepository.DeleteByPersonIdAsync(id, cancellationToken);
        await _personRepository.DeleteAsync(id, cancellationToken);
    }

    private static PersonResponse ToResponse(Person person)
        => new(person.Id, person.Name, person.Age, person.IsMinor);
}
