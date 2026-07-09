namespace ControleGastos.Domain.Exceptions;

/// <summary>
/// Exceção base para todas as violações de regras de negócio do domínio.
/// Serve como "marcador" para que camadas superiores (API) saibam
/// traduzir essas falhas em respostas HTTP apropriadas (400/404/422),
/// sem que o domínio precise conhecer nada sobre HTTP.
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }
}

/// <summary>
/// Lançada quando uma entidade obrigatória não é encontrada (ex.: pessoa inexistente).
/// </summary>
public sealed class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, object id)
        : base($"{entityName} com identificador '{id}' não foi encontrado(a).")
    {
    }
}

/// <summary>
/// Lançada quando uma regra de negócio é violada
/// (ex.: dados inválidos, menor de idade cadastrando receita).
/// </summary>
public sealed class BusinessRuleViolationException : DomainException
{
    public BusinessRuleViolationException(string message) : base(message)
    {
    }
}
