import type { Person, Transaction, TotalsReport, ApiErrorPayload } from "./types";

/**
 * URL base da API. Pode ser customizada em tempo de build através da variável
 * de ambiente VITE_API_BASE_URL (ver arquivo .env.example), sem precisar alterar código.
 */
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5182/api";

/** Erro de aplicação com a mensagem já traduzida a partir da resposta da API. */
export class ApiError extends Error {}

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  let response: Response;

  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      headers: { "Content-Type": "application/json" },
      ...options,
    });
  } catch {
    throw new ApiError(
      "Não foi possível conectar à API. Verifique se o back-end está em execução."
    );
  }

  if (response.status === 204) {
    return undefined as T;
  }

  const isJson = response.headers.get("content-type")?.includes("application/json");
  const body = isJson ? await response.json() : undefined;

  if (!response.ok) {
    const message = (body as ApiErrorPayload | undefined)?.error ?? "Erro inesperado na API.";
    throw new ApiError(message);
  }

  return body as T;
}

export const api = {
  people: {
    list: () => request<Person[]>("/people"),
    create: (data: { name: string; age: number }) =>
      request<Person>("/people", { method: "POST", body: JSON.stringify(data) }),
    remove: (id: string) => request<void>(`/people/${id}`, { method: "DELETE" }),
  },
  transactions: {
    list: (personId?: string) =>
      request<Transaction[]>(personId ? `/transactions?personId=${personId}` : "/transactions"),
    create: (data: { description: string; value: number; type: string; personId: string }) =>
      request<Transaction>("/transactions", { method: "POST", body: JSON.stringify(data) }),
  },
  totals: {
    get: () => request<TotalsReport>("/totals"),
  },
};
