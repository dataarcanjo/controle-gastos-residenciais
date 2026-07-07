import { useMemo, useState } from "react";
import type { FormEvent } from "react";
import { useData } from "../context/DataContext";
import { ApiError } from "../api";
import { formatCurrency } from "../format";
import "./TransactionsSection.css";

interface TransactionsSectionProps {
  onError: (message: string) => void;
}

export function TransactionsSection({ onError }: TransactionsSectionProps) {
  const { people, transactions, createTransaction } = useData();
  const [description, setDescription] = useState("");
  const [value, setValue] = useState("");
  const [type, setType] = useState<"Despesa" | "Receita">("Despesa");
  const [personId, setPersonId] = useState("");
  const [submitting, setSubmitting] = useState(false);

  const selectedPerson = useMemo(
    () => people.find((p) => p.id === personId) ?? null,
    [people, personId]
  );
  const onlyExpenseAllowed = selectedPerson?.isMinor ?? false;

  const peopleById = useMemo(() => new Map(people.map((p) => [p.id, p])), [people]);

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    const parsedValue = Number(value.replace(",", "."));

    if (!description.trim()) {
      onError("Informe a descrição da transação.");
      return;
    }
    if (!personId) {
      onError("Selecione a pessoa responsável pela transação.");
      return;
    }
    if (!Number.isFinite(parsedValue) || parsedValue <= 0) {
      onError("Informe um valor maior que zero.");
      return;
    }

    setSubmitting(true);
    try {
      await createTransaction({ description: description.trim(), value: parsedValue, type, personId });
      setDescription("");
      setValue("");
    } catch (err) {
      onError(err instanceof ApiError ? err.message : "Não foi possível cadastrar a transação.");
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="section">
      <header className="section__header">
        <h2>Transações</h2>
        <p className="section__hint">
          Registre despesas e receitas. Uma pessoa precisa estar cadastrada antes de receber transações.
        </p>
      </header>

      {people.length === 0 ? (
        <p className="empty-state">Cadastre ao menos uma pessoa antes de lançar transações.</p>
      ) : (
        <form className="inline-form" onSubmit={handleSubmit}>
          <label className="field">
            <span>Descrição</span>
            <input
              type="text"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="Ex.: Conta de luz"
              maxLength={200}
            />
          </label>

          <label className="field field--narrow">
            <span>Valor (R$)</span>
            <input
              type="text"
              inputMode="decimal"
              value={value}
              onChange={(e) => setValue(e.target.value)}
              placeholder="0,00"
            />
          </label>

          <label className="field field--narrow">
            <span>Pessoa</span>
            <select value={personId} onChange={(e) => setPersonId(e.target.value)}>
              <option value="">Selecione…</option>
              {people.map((person) => (
                <option key={person.id} value={person.id}>
                  {person.name}
                </option>
              ))}
            </select>
          </label>

          <label className="field field--narrow">
            <span>Tipo</span>
            <select
              value={type}
              onChange={(e) => setType(e.target.value as "Despesa" | "Receita")}
            >
              <option value="Despesa">Despesa</option>
              <option value="Receita" disabled={onlyExpenseAllowed}>
                Receita
              </option>
            </select>
          </label>

          <button type="submit" className="btn btn--primary" disabled={submitting}>
            {submitting ? "Lançando…" : "Lançar transação"}
          </button>

          {onlyExpenseAllowed && (
            <p className="inline-form__notice">
              {selectedPerson?.name} é menor de idade: apenas despesas podem ser lançadas.
            </p>
          )}
        </form>
      )}

      {transactions.length === 0 ? (
        <p className="empty-state">Nenhuma transação lançada ainda.</p>
      ) : (
        <div className="table-scroll">
        <table className="ledger-table">
          <thead>
            <tr>
              <th>Descrição</th>
              <th>Pessoa</th>
              <th>Tipo</th>
              <th className="ledger-table__value">Valor</th>
            </tr>
          </thead>
          <tbody>
            {transactions.map((transaction) => (
              <tr key={transaction.id}>
                <td>{transaction.description}</td>
                <td>{peopleById.get(transaction.personId)?.name ?? "—"}</td>
                <td>
                  <span
                    className={`type-tag ${transaction.type === "Receita" ? "type-tag--income" : "type-tag--expense"}`}
                  >
                    {transaction.type}
                  </span>
                </td>
                <td className={`ledger-table__value tabular ${transaction.type === "Receita" ? "value--income" : "value--expense"}`}>
                  {transaction.type === "Receita" ? "+" : "−"} {formatCurrency(transaction.value)}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        </div>
      )}
    </div>
  );
}
