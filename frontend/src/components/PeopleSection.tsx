import { useState } from "react";
import type { FormEvent } from "react";
import { useData } from "../context/DataContext";
import { ApiError } from "../api";
import "./PeopleSection.css";

interface PeopleSectionProps {
  onError: (message: string) => void;
}

export function PeopleSection({ onError }: PeopleSectionProps) {
  const { people, createPerson, deletePerson } = useData();
  const [name, setName] = useState("");
  const [age, setAge] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const [pendingDeleteId, setPendingDeleteId] = useState<string | null>(null);

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    const parsedAge = Number(age);

    if (!name.trim()) {
      onError("Informe o nome da pessoa.");
      return;
    }
    if (!Number.isInteger(parsedAge) || parsedAge < 0) {
      onError("Informe uma idade válida.");
      return;
    }

    setSubmitting(true);
    try {
      await createPerson(name.trim(), parsedAge);
      setName("");
      setAge("");
    } catch (err) {
      onError(err instanceof ApiError ? err.message : "Não foi possível cadastrar a pessoa.");
    } finally {
      setSubmitting(false);
    }
  }

  async function handleDelete(id: string) {
    try {
      await deletePerson(id);
      setPendingDeleteId(null);
    } catch (err) {
      onError(err instanceof ApiError ? err.message : "Não foi possível remover a pessoa.");
    }
  }

  return (
    <div className="section">
      <header className="section__header">
        <h2>Pessoas</h2>
        <p className="section__hint">
          Cadastre quem mora na casa. Pessoas com menos de 18 anos só podem registrar despesas.
        </p>
      </header>

      <form className="inline-form" onSubmit={handleSubmit}>
        <label className="field">
          <span>Nome</span>
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="Ex.: Ana Souza"
            maxLength={120}
          />
        </label>
        <label className="field field--narrow">
          <span>Idade</span>
          <input
            type="number"
            value={age}
            onChange={(e) => setAge(e.target.value)}
            placeholder="Ex.: 34"
            min={0}
            max={130}
          />
        </label>
        <button type="submit" className="btn btn--primary" disabled={submitting}>
          {submitting ? "Cadastrando…" : "Cadastrar pessoa"}
        </button>
      </form>

      {people.length === 0 ? (
        <p className="empty-state">Nenhuma pessoa cadastrada ainda. Adicione a primeira acima.</p>
      ) : (
        <ul className="people-grid">
          {people.map((person) => (
            <li key={person.id} className="person-card">
              <div className="person-card__main">
                <span className="person-card__name">{person.name}</span>
                <span className="person-card__age tabular">{person.age} anos</span>
              </div>
              {person.isMinor && <span className="badge badge--minor">apenas despesas · menor de idade</span>}

              {pendingDeleteId === person.id ? (
                <div className="confirm-row">
                  <span>Remover e apagar todas as transações desta pessoa?</span>
                  <div className="confirm-row__actions">
                    <button type="button" className="btn btn--danger" onClick={() => handleDelete(person.id)}>
                      Confirmar
                    </button>
                    <button type="button" className="btn btn--ghost" onClick={() => setPendingDeleteId(null)}>
                      Cancelar
                    </button>
                  </div>
                </div>
              ) : (
                <button
                  type="button"
                  className="btn btn--ghost person-card__delete"
                  onClick={() => setPendingDeleteId(person.id)}
                >
                  Remover
                </button>
              )}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
