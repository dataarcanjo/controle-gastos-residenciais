import { useData } from "../context/DataContext";
import { formatCurrency } from "../format";
import { Stamp } from "./Stamp";
import "./TotalsSection.css";

export function TotalsSection() {
  const { totals } = useData();

  if (!totals || totals.people.length === 0) {
    return (
      <div className="section">
        <header className="section__header">
          <h2>Totais</h2>
          <p className="section__hint">Receitas, despesas e saldo por pessoa, e o total geral da casa.</p>
        </header>
        <p className="empty-state">Cadastre pessoas e transações para ver os totais aqui.</p>
      </div>
    );
  }

  return (
    <div className="section">
      <header className="section__header">
        <h2>Totais</h2>
        <p className="section__hint">Receitas, despesas e saldo por pessoa, e o total geral da casa.</p>
      </header>

      <div className="table-scroll">
      <table className="ledger-table totals-table">
        <thead>
          <tr>
            <th>Pessoa</th>
            <th className="ledger-table__value">Receitas</th>
            <th className="ledger-table__value">Despesas</th>
            <th className="ledger-table__value">Saldo</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {totals.people.map((person) => (
            <tr key={person.personId}>
              <td>{person.name}</td>
              <td className="ledger-table__value tabular value--income">{formatCurrency(person.totalIncome)}</td>
              <td className="ledger-table__value tabular value--expense">{formatCurrency(person.totalExpense)}</td>
              <td className={`ledger-table__value tabular ${person.balance >= 0 ? "value--income" : "value--expense"}`}>
                {formatCurrency(person.balance)}
              </td>
              <td>
                <Stamp positive={person.balance >= 0} />
              </td>
            </tr>
          ))}
        </tbody>
        <tfoot>
          <tr className="totals-table__grand-row">
            <td>Total geral</td>
            <td className="ledger-table__value tabular value--income">{formatCurrency(totals.grandTotalIncome)}</td>
            <td className="ledger-table__value tabular value--expense">{formatCurrency(totals.grandTotalExpense)}</td>
            <td
              className={`ledger-table__value tabular ${totals.grandTotalBalance >= 0 ? "value--income" : "value--expense"}`}
            >
              {formatCurrency(totals.grandTotalBalance)}
            </td>
            <td>
              <Stamp positive={totals.grandTotalBalance >= 0} />
            </td>
          </tr>
        </tfoot>
      </table>
      </div>
    </div>
  );
}
