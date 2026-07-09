using ControleGastos.Application.DTOs;
using ControleGastos.Application.Interfaces;
using ControleGastos.Domain.Enums;
using ControleGastos.Domain.Interfaces;

namespace ControleGastos.Application.Services;

/// <summary>
/// Implementação da consulta de totais: receitas, despesas e saldo por pessoa,
/// além do total geral consolidado.
/// </summary>
public class TotalsService : ITotalsService
{
    private readonly IPersonRepository _personRepository;
    private readonly ITransactionRepository _transactionRepository;

    public TotalsService(IPersonRepository personRepository, ITransactionRepository transactionRepository)
    {
        _personRepository = personRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<TotalsReportResponse> GetReportAsync(CancellationToken cancellationToken = default)
    {
        var people = await _personRepository.GetAllAsync(cancellationToken);
        var transactions = await _transactionRepository.GetAllAsync(cancellationToken);

        var transactionsByPerson = transactions
            .GroupBy(t => t.PersonId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var peopleTotals = new List<PersonTotalResponse>();

        foreach (var person in people)
        {
            transactionsByPerson.TryGetValue(person.Id, out var personTransactions);
            personTransactions ??= new();

            var income = personTransactions
                .Where(t => t.Type == TransactionType.Receita)
                .Sum(t => t.Value);

            var expense = personTransactions
                .Where(t => t.Type == TransactionType.Despesa)
                .Sum(t => t.Value);

            peopleTotals.Add(new PersonTotalResponse(
                PersonId: person.Id,
                Name: person.Name,
                TotalIncome: income,
                TotalExpense: expense,
                Balance: income - expense));
        }

        var grandTotalIncome = peopleTotals.Sum(p => p.TotalIncome);
        var grandTotalExpense = peopleTotals.Sum(p => p.TotalExpense);

        return new TotalsReportResponse(
            People: peopleTotals,
            GrandTotalIncome: grandTotalIncome,
            GrandTotalExpense: grandTotalExpense,
            GrandTotalBalance: grandTotalIncome - grandTotalExpense);
    }
}
