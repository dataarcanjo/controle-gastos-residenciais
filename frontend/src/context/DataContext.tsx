import { createContext, useCallback, useContext, useEffect, useMemo, useState } from "react";
import type { ReactNode } from "react";
import { api, ApiError } from "../api";
import type { Person, Transaction, TotalsReport } from "../types";

interface DataContextValue {
  people: Person[];
  transactions: Transaction[];
  totals: TotalsReport | null;
  loading: boolean;
  createPerson: (name: string, age: number) => Promise<void>;
  deletePerson: (id: string) => Promise<void>;
  createTransaction: (input: {
    description: string;
    value: number;
    type: string;
    personId: string;
  }) => Promise<void>;
}

const DataContext = createContext<DataContextValue | null>(null);

/**
 * Centraliza o estado vindo da API (pessoas, transações e totais) e as ações
 * que o modificam, para que as diferentes seções da tela não dupliquem
 * chamadas nem fiquem fora de sincronia entre si (ex.: criar uma transação
 * também precisa atualizar a consulta de totais).
 */
export function DataProvider({ children }: { children: ReactNode }) {
  const [people, setPeople] = useState<Person[]>([]);
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [totals, setTotals] = useState<TotalsReport | null>(null);
  const [loading, setLoading] = useState(true);

  const refreshAll = useCallback(async () => {
    const [peopleData, transactionsData, totalsData] = await Promise.all([
      api.people.list(),
      api.transactions.list(),
      api.totals.get(),
    ]);
    setPeople(peopleData);
    setTransactions(transactionsData);
    setTotals(totalsData);
  }, []);

  useEffect(() => {
    setLoading(true);
    refreshAll().finally(() => setLoading(false));
  }, [refreshAll]);

  const createPerson = useCallback(
    async (name: string, age: number) => {
      await api.people.create({ name, age });
      await refreshAll();
    },
    [refreshAll]
  );

  const deletePerson = useCallback(
    async (id: string) => {
      await api.people.remove(id);
      await refreshAll();
    },
    [refreshAll]
  );

  const createTransaction = useCallback(
    async (input: { description: string; value: number; type: string; personId: string }) => {
      await api.transactions.create(input);
      await refreshAll();
    },
    [refreshAll]
  );

  const value = useMemo<DataContextValue>(
    () => ({ people, transactions, totals, loading, createPerson, deletePerson, createTransaction }),
    [people, transactions, totals, loading, createPerson, deletePerson, createTransaction]
  );

  return <DataContext.Provider value={value}>{children}</DataContext.Provider>;
}

export function useData(): DataContextValue {
  const ctx = useContext(DataContext);
  if (!ctx) {
    throw new Error("useData deve ser usado dentro de um DataProvider.");
  }
  return ctx;
}

export { ApiError };
