import { getAccessToken, setAccessToken } from "./auth/tokenStore";
import { AuthResult } from "./types";

const BASE_URL = "http://localhost:8000";

export class ApiError extends Error {
  status: number;
  constructor(message: string, status: number) {
    super(message);
    this.status = status;
  }
}

// GET /api/auth/refresh — returns the AuthResult when a new token was issued, else null.
export async function refreshSession(): Promise<AuthResult | null> {
  const res = await fetch(`${BASE_URL}/api/auth/refresh`, {
    method: "GET",
    credentials: "include",
  });
  if (!res.ok) return null;
  const result: AuthResult = await res.json();
  if (result.Token) setAccessToken(result.Token);
  return result;
}

export async function api<T>(
  path: string,
  opts: { method?: string; body?: unknown; auth?: boolean } = {},
): Promise<T> {
  const { method = "GET", body, auth = true } = opts;
  const headers: Record<string, string> = {};
  if (body !== undefined) headers["Content-Type"] = "application/json";
  const token = getAccessToken();
  if (auth && token) headers["Authorization"] = `Bearer ${token}`;

  let res = await fetch(`${BASE_URL}${path}`, {
    method,
    headers,
    body: body !== undefined ? JSON.stringify(body) : undefined,
    credentials: "include",
  });

  if (res.status === 401 && auth) {
    const result = await refreshSession();
    if (result?.Token) {
      const headers2: Record<string, string> = {};
      if (body !== undefined) headers2["Content-Type"] = "application/json";
      headers2["Authorization"] = `Bearer ${result.Token}`;
      res = await fetch(`${BASE_URL}${path}`, {
        method,
        headers: headers2,
        body: body !== undefined ? JSON.stringify(body) : undefined,
        credentials: "include",
      });
    }
  }

  if (!res.ok) {
    throw new ApiError(await res.text(), res.status);
  }
  const text = await res.text();
  if (!text) return undefined as T;
  return JSON.parse(text) as T;
}