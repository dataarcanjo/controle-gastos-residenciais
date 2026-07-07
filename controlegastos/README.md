# Sistema de Controle de Gastos Residenciais — Back-end

API REST em **.NET 8 / C#** para controle de gastos residenciais: cadastro de pessoas, cadastro de transações (despesas/receitas) e consulta de totais.

## Arquitetura

O projeto segue uma arquitetura em camadas (Clean Architecture simplificada), com o objetivo de manter as regras de negócio isoladas de detalhes de infraestrutura e fáceis de testar/evoluir:

```
src/
├── ControleGastos.Domain          → Entidades, regras de negócio e contratos (interfaces)
│                                     Não depende de nenhuma outra camada.
├── ControleGastos.Application     → Casos de uso (services), DTOs
│                                     Depende apenas de Domain.
├── ControleGastos.Infrastructure  → Persistência (implementação dos repositórios)
│                                     Depende de Domain e Application.
└── ControleGastos.Api             → Controllers, middleware de erros, composição da aplicação
                                      Depende de Application e Infrastructure.
```

A regra de dependência é sempre "de fora para dentro": `Api → Infrastructure → Application → Domain`. O `Domain` nunca conhece as camadas acima dele.

Essa separação é o que torna a solução **escalável**: para trocar a forma de persistência (por exemplo, para Entity Framework Core + PostgreSQL/SQL Server), basta criar novas implementações de `IPersonRepository`/`ITransactionRepository` dentro de `Infrastructure` — nada em `Domain`, `Application` ou `Api` precisa mudar. O mesmo vale para adicionar novas regras de negócio, novos casos de uso ou uma nova forma de expor a API (ex.: gRPC), sem reescrever o que já existe.

## Persistência dos dados

Os dados são persistidos em um arquivo JSON local (`App_Data/data.json`, criado automaticamente na primeira execução), garantindo que os dados sobrevivam ao fechamento da aplicação, conforme exigido. O acesso ao arquivo é protegido por lock assíncrono para evitar corrupção em acessos concorrentes, e a escrita é feita em arquivo temporário + substituição atômica.

A camada de acesso a dados foi propositalmente isolada atrás das interfaces de repositório (`Domain.Interfaces`), então trocar esse mecanismo por um banco de dados relacional de verdade é uma mudança restrita à camada `Infrastructure`.

## Tecnologias

- .NET 8 / C#
- ASP.NET Core Web API (controllers)
- Persistência em arquivo JSON via `System.Text.Json` (sem dependências externas de terceiros)

## Como executar

Pré-requisito: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
cd src/ControleGastos.Api
dotnet run
```

A API sobe por padrão em `http://localhost:5182`. Os dados ficam em `src/ControleGastos.Api/bin/Debug/net8.0/App_Data/data.json`.

Para customizar o caminho do arquivo de dados, defina a configuração `Persistence:DataFilePath` (via `appsettings.json`, variável de ambiente `Persistence__DataFilePath`, etc.).

## Endpoints

### Pessoas — `/api/people`

| Método | Rota              | Descrição                                              |
|--------|-------------------|----------------------------------------------------------|
| POST   | `/api/people`     | Cadastra uma pessoa. Body: `{ "name": string, "age": int }` |
| GET    | `/api/people`     | Lista todas as pessoas cadastradas                        |
| DELETE | `/api/people/{id}`| Remove uma pessoa **e todas as suas transações** (cascata) |

### Transações — `/api/transactions`

| Método | Rota                              | Descrição |
|--------|-----------------------------------|-----------|
| POST   | `/api/transactions`               | Cadastra uma transação. Body: `{ "description": string, "value": decimal, "type": "Despesa" \| "Receita", "personId": guid }` |
| GET    | `/api/transactions`               | Lista todas as transações |
| GET    | `/api/transactions?personId={id}` | Lista as transações de uma pessoa específica |

### Totais — `/api/totals`

| Método | Rota          | Descrição |
|--------|---------------|-----------|
| GET    | `/api/totals` | Lista, para cada pessoa, o total de receitas, despesas e saldo; e ao final, o total geral consolidado |

## Regras de negócio implementadas

- Identificadores de pessoas e transações são `Guid`s gerados automaticamente.
- Ao cadastrar uma transação, a pessoa informada **precisa existir** (senão, `404 Not Found`).
- Pessoas **menores de 18 anos só podem ter despesas cadastradas** — tentar cadastrar uma receita para um menor retorna `400 Bad Request` com uma mensagem explicativa.
- Ao deletar uma pessoa, **todas as suas transações são removidas em cascata**.
- Nome, idade, descrição e valor são validados nas entidades de domínio (ex.: valor deve ser maior que zero).

## Tratamento de erros

Um middleware central (`ExceptionHandlingMiddleware`) converte as exceções de domínio em respostas HTTP padronizadas:

- `EntityNotFoundException` → `404 Not Found`
- `BusinessRuleViolationException` → `400 Bad Request`
- Qualquer outro erro não tratado → `500 Internal Server Error`

Todas seguem o formato `{ "error": "mensagem" }`.

## Exemplos rápidos (curl)

```bash
# Cadastrar pessoa
curl -X POST http://localhost:5182/api/people -H "Content-Type: application/json" -d '{"name":"Maria","age":30}'

# Cadastrar transação
curl -X POST http://localhost:5182/api/transactions -H "Content-Type: application/json" -d '{"description":"Salário","value":5000,"type":"Receita","personId":"<id-da-pessoa>"}'

# Consultar totais
curl http://localhost:5182/api/totals
```
