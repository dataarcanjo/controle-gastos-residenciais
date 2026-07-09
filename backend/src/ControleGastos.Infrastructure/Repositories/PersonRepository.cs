using ControleGastos.Domain.Entities;
using ControleGastos.Domain.Interfaces;
using ControleGastos.Infrastructure.Persistence;

namespace ControleGastos.Infrastructure.Repositories;

public sealed class PersonRepository : IPersonRepository
{
    private readonly JsonDataStore _store;

    public PersonRepository(JsonDataStore store)
    {
        _store = store;
    }

    public Task<Person?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _store.ReadAsync(data =>
        {
            var record = data.People.FirstOrDefault(p => p.Id == id);
            return record is null ? null : ToDomain(record);
        }, cancellationToken);
    }

    public Task<IReadOnlyList<Person>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _store.ReadAsync<IReadOnlyList<Person>>(data =>
            data.People.Select(ToDomain).ToList(), cancellationToken);
    }

    public Task AddAsync(Person person, CancellationToken cancellationToken = default)
    {
        return _store.WriteAsync(data =>
        {
            data.People.Add(new PersonRecord
            {
                Id = person.Id,
                Name = person.Name,
                Age = person.Age
            });
        }, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var removed = false;

        await _store.WriteAsync(data =>
        {
            removed = data.People.RemoveAll(p => p.Id == id) > 0;
        }, cancellationToken);

        return removed;
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _store.ReadAsync(data => data.People.Any(p => p.Id == id), cancellationToken);
    }

    private static Person ToDomain(PersonRecord record)
        => Person.Restore(record.Id, record.Name, record.Age);
}
