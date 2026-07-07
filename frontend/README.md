# Livro-Caixa — Front-end (Controle de Gastos Residenciais)

Front-end em **React + TypeScript** (Vite) para o sistema de controle de gastos residenciais: cadastro de pessoas, cadastro de transações e consulta de totais. Consome a API do back-end (`ControleGastos.Api`).

## Tecnologias

- React 19 + TypeScript
- Vite
- CSS puro (sem framework de UI), com um pequeno sistema de tokens de design (cores, tipografia) em `src/index.css`

Nenhuma biblioteca de UI/estado externa foi usada — o estado da aplicação é centralizado em um Context (`src/context/DataContext.tsx`) que conversa com a API através de `src/api.ts`.

## Pré-requisitos

- [Node.js](https://nodejs.org) 18+ (recomendado: LTS)
- O back-end (`ControleGastos.Api`) rodando — por padrão em `http://localhost:5182`

## Como executar

```bash
npm install
npm run dev
```

A aplicação sobe por padrão em `http://localhost:5173`.

### Apontando para outro endereço da API

Por padrão, o front-end consome a API em `http://localhost:5182/api`. Para customizar:

```bash
cp .env.example .env
# edite VITE_API_BASE_URL em .env
```

## Build de produção

```bash
npm run build   # gera a pasta dist/
npm run preview # serve o build de produção localmente, para conferência
```

## Estrutura do projeto

```
src/
├── api.ts                     → cliente HTTP da API (fetch + tratamento de erros)
├── types.ts                   → tipos TypeScript espelhando os DTOs do back-end
├── format.ts                  → formatação de moeda (BRL)
├── context/
│   └── DataContext.tsx        → estado global: pessoas, transações, totais + ações
└── components/
    ├── PeopleSection.tsx      → cadastro/listagem/remoção de pessoas
    ├── TransactionsSection.tsx→ cadastro/listagem de transações
    ├── TotalsSection.tsx      → consulta de totais por pessoa + total geral
    ├── Stamp.tsx               → "carimbo" de saldo positivo/negativo
    └── Notice.tsx              → banner de erro/sucesso
```

## Funcionalidades e regras de negócio refletidas na UI

- **Pessoas**: cadastro (nome, idade), listagem em cartões e remoção com confirmação explícita (a remoção apaga também as transações da pessoa, conforme a regra de negócio do back-end).
- Pessoas **menores de 18 anos** exibem um selo "apenas despesas" e, ao lançar uma transação para elas, a opção "Receita" fica desabilitada no formulário — a mesma regra é validada novamente no back-end.
- **Transações**: formulário de lançamento (descrição, valor, tipo, pessoa) e listagem em formato de tabela, com destaque visual (cor) para receitas e despesas.
- **Totais**: tabela com receitas, despesas e saldo por pessoa, um "carimbo" indicando se o saldo está positivo ou negativo, e uma linha de total geral consolidado — também exibido de forma resumida na faixa superior (masthead) em todas as telas.
- Mensagens de erro vindas da API (ex.: tentar cadastrar receita para menor de idade, ou pessoa inexistente) são exibidas em um banner no topo do conteúdo.

## Identidade visual

O visual remete a livros-caixa e passbooks bancários físicos: papel claro pautado, tipografia serifada (Newsreader) para títulos e números, tipografia monoespaçada (IBM Plex Mono) para valores monetários (alinhamento tabular), e um "carimbo" de tinta (latão para saldo positivo, vermelho para saldo negativo) como elemento de assinatura visual — reforçando a metáfora contábil de "estar no vermelho".
