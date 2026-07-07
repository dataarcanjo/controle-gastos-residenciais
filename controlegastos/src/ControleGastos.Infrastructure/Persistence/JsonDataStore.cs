using System.Text.Json;

namespace ControleGastos.Infrastructure.Persistence;

/// <summary>
/// Armazenamento simples em arquivo JSON, registrado como singleton na aplicação.
/// <para>
/// Por que arquivo em vez de um banco de dados "de verdade" (SQLite/SQL Server/etc.)?
/// Toda a lógica de negócio depende apenas das interfaces <c>IPersonRepository</c> e
/// <c>ITransactionRepository</c> (definidas em ControleGastos.Domain), nunca deste tipo diretamente.
/// Isso significa que trocar este arquivo por Entity Framework Core + um banco relacional
/// é uma mudança isolada nesta camada (ControleGastos.Infrastructure) — bastaria criar novas
/// implementações de repositório — sem tocar em Domain, Application ou Api.
/// Essa é a característica que torna a solução escalável apesar da simplicidade do armazenamento.
/// </para>
/// <para>
/// Concorrência: todo acesso (leitura ou escrita) passa por um <see cref="SemaphoreSlim"/>
/// assíncrono, garantindo que requisições concorrentes não corrompam o arquivo nem leiam
/// dados inconsistentes a meio de uma escrita.
/// </para>
/// </summary>
public sealed class JsonDataStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly string _filePath;
    private DataModel? _cache;

    public JsonDataStore(string filePath)
    {
        _filePath = filePath;
    }

    /// <summary>Executa uma leitura consistente sobre os dados atuais.</summary>
    public async Task<T> ReadAsync<T>(Func<DataModel, T> read, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            var data = await EnsureLoadedAsync(cancellationToken);
            return read(data);
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>Executa uma mutação e persiste o resultado em disco antes de liberar o lock.</summary>
    public async Task WriteAsync(Action<DataModel> mutate, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            var data = await EnsureLoadedAsync(cancellationToken);
            mutate(data);
            await PersistAsync(data, cancellationToken);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<DataModel> EnsureLoadedAsync(CancellationToken cancellationToken)
    {
        if (_cache is not null)
        {
            return _cache;
        }

        if (!File.Exists(_filePath))
        {
            _cache = new DataModel();
            return _cache;
        }

        await using var stream = File.OpenRead(_filePath);
        _cache = await JsonSerializer.DeserializeAsync<DataModel>(stream, SerializerOptions, cancellationToken)
                 ?? new DataModel();
        return _cache;
    }

    private async Task PersistAsync(DataModel data, CancellationToken cancellationToken)
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Escreve em um arquivo temporário e substitui o original de forma atômica,
        // evitando deixar o arquivo de dados corrompido caso o processo seja
        // interrompido no meio de uma escrita.
        var tempPath = _filePath + ".tmp";
        await using (var stream = File.Create(tempPath))
        {
            await JsonSerializer.SerializeAsync(stream, data, SerializerOptions, cancellationToken);
        }

        File.Move(tempPath, _filePath, overwrite: true);
        _cache = data;
    }
}
