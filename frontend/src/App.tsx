import { useState } from "react";
import { DataProvider, useData } from "./context/DataContext";
import { PeopleSection } from "./components/PeopleSection";
import { TransactionsSection } from "./components/TransactionsSection";
import { TotalsSection } from "./components/TotalsSection";
import { Notice } from "./components/Notice";
import { formatCurrency } from "./format";
import "./components/shared.css";
import "./App.css";

type Section = "pessoas" | "transacoes" | "totais";

const SECTIONS: { id: Section; label: string }[] = [
  { id: "pessoas", label: "Pessoas" },
  { id: "transacoes", label: "Transações" },
  { id: "totais", label: "Totais" },
];

function AppContent() {
  const { totals, loading } = useData();
  const [section, setSection] = useState<Section>("pessoas");
  const [error, setError] = useState<string | null>(null);

  return (
    <div className="app">
      <header className="masthead">
        <div className="masthead__title">
          <h1>Livro-Caixa</h1>
          <p>Controle de gastos residenciais</p>
        </div>

        {totals && (
          <dl className="masthead__ribbon" aria-label="Totais gerais da casa">
            <div className="ribbon-stat">
              <dt>Receitas</dt>
              <dd className="tabular value--income">{formatCurrency(totals.grandTotalIncome)}</dd>
            </div>
            <div className="ribbon-stat">
              <dt>Despesas</dt>
              <dd className="tabular value--expense">{formatCurrency(totals.grandTotalExpense)}</dd>
            </div>
            <div className="ribbon-stat ribbon-stat--balance">
              <dt>Saldo</dt>
              <dd className={`tabular ${totals.grandTotalBalance >= 0 ? "value--income" : "value--expense"}`}>
                {formatCurrency(totals.grandTotalBalance)}
              </dd>
            </div>
          </dl>
        )}
      </header>

      <div className="layout">
        <nav className="ledger-tabs" aria-label="Seções">
          {SECTIONS.map((tab) => (
            <button
              key={tab.id}
              type="button"
              className={`ledger-tab ${section === tab.id ? "ledger-tab--active" : ""}`}
              onClick={() => setSection(tab.id)}
              aria-current={section === tab.id ? "page" : undefined}
            >
              {tab.label}
            </button>
          ))}
        </nav>

        <main className="content">
          {error && <Notice kind="error" message={error} onDismiss={() => setError(null)} />}

          {loading ? (
            <p className="empty-state">Carregando dados…</p>
          ) : (
            <>
              {section === "pessoas" && <PeopleSection onError={setError} />}
              {section === "transacoes" && <TransactionsSection onError={setError} />}
              {section === "totais" && <TotalsSection />}
            </>
          )}
        </main>
      </div>
    </div>
  );
}

function App() {
  return (
    <DataProvider>
      <AppContent />
    </DataProvider>
  );
}

export default App;
