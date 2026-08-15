"use client";

import { createContext, useContext, useEffect, useState, ReactNode } from "react";
import { api, refreshSession } from "../api";
import { setAccessToken } from "./tokenStore";
import { AuthResult, UserInfo } from "../types";

interface AuthState {
  user: UserInfo | null;
  token: string | null;
  loading: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (data: {
    Email: string;
    FirstName: string;
    LastName: string;
    Password: string;
    Role: "User" | "Author";
  }) => Promise<void>;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthState | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<UserInfo | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    (async () => {
      const result = await refreshSession();
      if (result?.Token && result.UserInfo) {
        setUser(result.UserInfo);
        setToken(result.Token);
      }
      setLoading(false);
    })();
  }, []);

  const apply = (result: AuthResult) => {
    if (result.Token) setAccessToken(result.Token);
    setToken(result.Token);
    setUser(result.UserInfo);
  };

  const login = async (email: string, password: string) => {
    apply(await api<AuthResult>("/api/auth/login", {
      method: "POST",
      body: { Email: email, Password: password },
      auth: false,
    }));
  };

  const register = async (data: {
    Email: string;
    FirstName: string;
    LastName: string;
    Password: string;
    Role: "User" | "Author";
  }) => {
    apply(await api<AuthResult>("/api/auth/register", {
      method: "POST",
      body: data,
      auth: false,
    }));
  };

  const logout = async () => {
    try {
      await api<void>("/api/auth", { method: "GET", auth: false });
    } finally {
      setAccessToken(null);
      setToken(null);
      setUser(null);
    }
  };

  return (
    <AuthContext.Provider value={{ user, token, loading, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth(): AuthState {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within AuthProvider");
  return ctx;
}