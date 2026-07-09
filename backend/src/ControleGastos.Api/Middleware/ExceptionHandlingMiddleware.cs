using System.Net;
using System.Text.Json;
using ControleGastos.Domain.Exceptions;

namespace ControleGastos.Api.Middleware;

/// <summary>
/// Middleware central de tratamento de erros. Converte as exceções de domínio
/// (que carregam apenas significado de negócio, sem nenhum conhecimento de HTTP)
/// nos códigos de status e formato de resposta apropriados para a API,
/// evitando try/catch repetido em cada controller/action.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (EntityNotFoundException ex)
        {
            await WriteResponseAsync(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (BusinessRuleViolationException ex)
        {
            await WriteResponseAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado ao processar a requisição {Path}", context.Request.Path);
            await WriteResponseAsync(context, HttpStatusCode.InternalServerError,
                "Ocorreu um erro inesperado ao processar a requisição.");
        }
    }

    private static Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = JsonSerializer.Serialize(new { error = message });
        return context.Response.WriteAsync(payload);
    }
}
