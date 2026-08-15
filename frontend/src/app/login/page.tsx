"use client";

import { useState, FormEvent } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useAuth } from "../share/auth/AuthContext";

export default function LoginPage() {
  const { login } = useAuth();
  const router = useRouter();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError(null);
    setSubmitting(true);
    try {
      await login(email, password);
      router.push("/");
    } catch (err) {
      const message = err instanceof Error ? err.message : String(err);
      try {
        const parsed = JSON.parse(message) as { error?: { message?: string } };
        setError(parsed.error?.message ?? message);
      } catch {
        setError(message);
      }
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <main className="flex flex-1 items-center justify-center">
      <form
        onSubmit={onSubmit}
        className="flex w-full max-w-sm flex-col gap-4 rounded-sm border border-slate-200 p-8"
      >
        <h1 className="text-xl text-slate-800">Log in</h1>
        <label className="flex flex-col gap-1 text-sm text-slate-600">
          Email
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            className="rounded-sm border border-slate-300 px-3 py-2 text-slate-800 outline-none focus:border-slate-500"
          />
        </label>
        <label className="flex flex-col gap-1 text-sm text-slate-600">
          Password
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            className="rounded-sm border border-slate-300 px-3 py-2 text-slate-800 outline-none focus:border-slate-500"
          />
        </label>
        {error && <p className="text-sm text-red-600">{error}</p>}
        <button
          type="submit"
          disabled={submitting}
          className="rounded-sm bg-slate-800 py-2 text-sm text-white disabled:opacity-50"
        >
          {submitting ? "Logging in…" : "Log in"}
        </button>
        <p className="text-sm text-slate-600">
          No account?{" "}
          <Link href="/register" className="text-slate-800 underline">
            Register
          </Link>
        </p>
      </form>
    </main>
  );
}