namespace ControleGastos.Application.DTOs;

/// <summary>Dados de entrada para o cadastro de uma pessoa.</summary>
public sealed record CreatePersonRequest(string Name, int Age);

/// <summary>Representação de saída de uma pessoa.</summary>
public sealed record PersonResponse(Guid Id, string Name, int Age, bool IsMinor);
