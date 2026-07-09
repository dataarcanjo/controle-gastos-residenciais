using ControleGastos.Api.Middleware;
using ControleGastos.Application;
using ControleGastos.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// --- Injeção de dependência por camada ---
// Cada camada expõe seu próprio método de extensão (AddApplication / AddInfrastructure),
// mantendo o Program.cs simples e a composição da aplicação explícita e fácil de navegar.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();

// Política de CORS permissiva para facilitar a integração com o front-end (React) durante
// o desenvolvimento. Em um cenário real de produção, restringir aos domínios conhecidos.
const string corsPolicyName = "AllowFrontend";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Middleware central de tratamento de erros: traduz exceções de domínio em respostas HTTP.
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors(corsPolicyName);

app.UseAuthorization();

app.MapControllers();

app.Run();

// Necessário para permitir testes de integração via WebApplicationFactory<Program>.
public partial class Program
{
}
