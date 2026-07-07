# Livro-Caixa — Controle de Gastos Residenciais

Sistema full-stack para controle de gastos residenciais: cadastro de pessoas, cadastro de transações (despesas/receitas) e consulta de totais por pessoa e geral.

O repositório está organizado como um monorepo com duas partes independentes:

```
.
├── backend/    → API REST em .NET 8 / C#
└── frontend/   → Aplicação em React + TypeScript (Vite)
```

## Visão geral

| Camada     | Tecnologia                          | Pasta        |
|------------|--------------------------------------|--------------|
| Back-end   | .NET 8, ASP.NET Core Web API, C#     | `backend/`   |
| Front-end  | React 19, TypeScript, Vite           | `frontend/`  |
| Persistência | Arquivo JSON local (sem dependências externas) | `backend/src/ControleGastos.Api/App_Data/data.json` |

Os dois projetos são independentes e se comunicam apenas via HTTP (a API expõe endpoints REST em JSON, e o front-end os consome via `fetch`). Não há acoplamento de código entre eles.

## Funcionalidades

- **Cadastro de pessoas** — criação, listagem e remoção (a remoção apaga também as transações da pessoa, em cascata).
- **Cadastro de transações** — criação e listagem de despesas/receitas vinculadas a uma pessoa.
- **Consulta de totais** — receitas, despesas e saldo por pessoa, além do total geral consolidado da casa.
- **Regra de negócio central**: pessoas **menores de 18 anos só podem ter despesas cadastradas** — a tentativa de cadastrar uma receita para um menor é bloqueada tanto na API (400 Bad Request) quanto na interface (opção desabilitada no formulário).

## Como executar o projeto completo

Pré-requisitos:
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org) 18+ (recomendado: LTS)

> **Windows:** evite pastas com caracteres especiais no caminho (ex.: `C#`, espaços, acentos), pois algumas ferramentas de build (Vite/Rolldown) não lidam bem com isso.

### 1. Back-end

```bash
cd backend/src/ControleGastos.Api
dotnet run
```

Sobe em `http://localhost:5182`. Os dados persistem em `backend/src/ControleGastos.Api/bin/Debug/net8.0/App_Data/data.json` e sobrevivem ao fechamento da aplicação.

### 2. Front-end

Em outro terminal:

```bash
cd frontend
npm install
npm run dev
```

Sobe em `http://localhost:5173` e já consome a API em `http://localhost:5182/api` por padrão (configurável via `.env`, veja `frontend/.env.example`). O CORS já está liberado no back-end para isso.

Abra `http://localhost:5173` no navegador — a tela de "Pessoas" deve carregar. Cadastre uma pessoa para confirmar que a integração está funcionando.

## Arquitetura do back-end

Clean Architecture simplificada, em camadas com dependência sempre "de fora para dentro":

```
Api → Infrastructure → Application → Domain
```

```
backend/src/
├── ControleGastos.Domain          → Entidades, regras de negócio e contratos (interfaces)
├── ControleGastos.Application     → Casos de uso (services), DTOs
├── ControleGastos.Infrastructure  → Persistência (implementação dos repositórios)
└── ControleGastos.Api             → Controllers, middleware de erros, composição da aplicação
```

Essa separação torna a persistência trocável (ex.: para Entity Framework Core + PostgreSQL/SQL Server) sem alterar nenhuma regra de negócio — bastaria criar novas implementações de `IPersonRepository`/`ITransactionRepository` dentro de `Infrastructure`.

### Endpoints da API

| Método | Rota                               | Descrição |
|--------|-------------------------------------|-----------|
| POST   | `/api/people`                       | Cadastra uma pessoa — `{ "name": string, "age": int }` |
| GET    | `/api/people`                       | Lista todas as pessoas |
| DELETE | `/api/people/{id}`                  | Remove uma pessoa e todas as suas transações (cascata) |
| POST   | `/api/transactions`                 | Cadastra uma transação — `{ "description": string, "value": decimal, "type": "Despesa" \| "Receita", "personId": guid }` |
| GET    | `/api/transactions`                 | Lista todas as transações |
| GET    | `/api/transactions?personId={id}`   | Lista as transações de uma pessoa específica |
| GET    | `/api/totals`                       | Totais de receita/despesa/saldo por pessoa + total geral |

Erros seguem o formato `{ "error": "mensagem" }`, com `404` para entidade não encontrada e `400` para violação de regra de negócio.

Mais detalhes técnicos (validações, tratamento de erros, exemplos de `curl`): veja [`controlegastos/README.md`](controlegastos/README.md).

## Arquitetura do front-end

```
frontend/src/
├── api.ts                      → cliente HTTP da API (fetch + tratamento de erros)
├── types.ts                    → tipos TypeScript espelhando os DTOs do back-end
├── format.ts                   → formatação de moeda (BRL)
├── context/DataContext.tsx     → estado global: pessoas, transações, totais + ações
└── components/
    ├── PeopleSection.tsx       → cadastro/listagem/remoção de pessoas
    ├── TransactionsSection.tsx → cadastro/listagem de transações
    ├── TotalsSection.tsx       → consulta de totais por pessoa + total geral
    ├── Stamp.tsx               → "carimbo" de saldo positivo/negativo
    └── Notice.tsx              → banner de erro/sucesso
```

Sem bibliotecas de UI/estado externas: React puro + Context API, e CSS próprio com um pequeno sistema de tokens de design (`src/index.css`).

A identidade visual remete a livros-caixa e passbooks bancários físicos: papel pautado, tipografia serifada (Newsreader) para títulos e números, tipografia monoespaçada (IBM Plex Mono) para valores monetários, e um "carimbo" de tinta (latão para saldo positivo, vermelho para saldo negativo).

Mais detalhes técnicos: veja [`frontend/README.md`](frontend/README.md).

## Decisões técnicas relevantes

- **Persistência em arquivo JSON em vez de um banco de dados tradicional**: garante que os dados sobrevivam ao fechamento da aplicação (requisito do desafio) sem exigir a instalação de um SGBD. A camada de acesso a dados fica isolada atrás de interfaces de repositório, o que torna a migração para um banco relacional (EF Core + SQL Server/PostgreSQL, por exemplo) uma mudança restrita à camada de infraestrutura, sem impacto nas regras de negócio.
- **Validação da regra do menor de idade em duas camadas**: no back-end (fonte da verdade, sempre validada) e replicada na interface (para uma experiência de uso melhor, evitando que a pessoa usuária só descubra o erro após submeter o formulário).
