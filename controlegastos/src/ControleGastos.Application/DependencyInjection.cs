using ControleGastos.Application.Interfaces;
using ControleGastos.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ControleGastos.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<ITotalsService, TotalsService>();

        return services;
    }
}
