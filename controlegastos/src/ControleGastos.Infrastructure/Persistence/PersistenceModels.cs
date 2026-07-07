namespace ControleGastos.Infrastructure.Persistence;

/// <summary>
/// Modelos simples (POCOs) usados exclusivamente para serialização/desserialização em disco.
/// Ficam separados das entidades de domínio de propósito para que o domínio não tenha
/// nenhuma dependência de detalhes de persistência (System.Text.Json, etc.) — quem faz a
/// conversão entre um e outro são os repositórios, na fronteira da camada de infraestrutura.
/// </summary>
public sealed class PersonRecord
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
}

public sealed class TransactionRecord
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Type { get; set; } = string.Empty;
    public Guid PersonId { get; set; }
}

/// <summary>Estrutura raiz do arquivo JSON persistido em disco.</summary>
public sealed class DataModel
{
    public List<PersonRecord> People { get; set; } = new();
    public List<TransactionRecord> Transactions { get; set; } = new();
}
