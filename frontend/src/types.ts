/** Tipos espelhando os DTOs expostos pelo back-end (ControleGastos.Api). */

export interface Person {
  id: string;
  name: string;
  age: number;
  isMinor: boolean;
}

export type TransactionType = "Despesa" | "Receita";

export interface Transaction {
  id: string;
  description: string;
  value: number;
  type: TransactionType;
  personId: string;
}

export interface PersonTotal {
  personId: string;
  name: string;
  totalIncome: number;
  totalExpense: number;
  balance: number;
}

export interface TotalsReport {
  people: PersonTotal[];
  grandTotalIncome: number;
  grandTotalExpense: number;
  grandTotalBalance: number;
}

/** Formato de erro retornado pelo middleware de exceções do back-end. */
export interface ApiErrorPayload {
  error: string;
}
