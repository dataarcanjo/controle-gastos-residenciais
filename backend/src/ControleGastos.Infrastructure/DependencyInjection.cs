using ControleGastos.Domain.Interfaces;
using ControleGastos.Infrastructure.Persistence;
using ControleGastos.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ControleGastos.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registra a infraestrutura de persistência. O caminho do arquivo de dados pode ser
    /// customizado via configuração ("Persistence:DataFilePath"), o que facilita, por exemplo,
    /// apontar para um volume montado em produção/containers. Por padrão, os dados ficam em
    /// "App_Data/data.json" dentro do diretório da aplicação.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var dataFilePath = configuration["Persistence:DataFilePath"];

        if (string.IsNullOrWhiteSpace(dataFilePath))
        {
            dataFilePath = Path.Combine(AppContext.BaseDirectory, "App_Data", "data.json");
        }

        // Singleton: precisamos de uma única instância protegendo o acesso ao arquivo
        // com o mesmo lock para toda a aplicação.
        services.AddSingleton(_ => new JsonDataStore(dataFilePath));

        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        return services;
    }
}
