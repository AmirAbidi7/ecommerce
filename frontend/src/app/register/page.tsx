"use client";

import { useState, FormEvent } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useAuth } from "../share/auth/AuthContext";

export default function RegisterPage() {
  const { register } = useAuth();
  const router = useRouter();
  const [email, setEmail] = useState("");
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [password, setPassword] = useState("");
  const [role, setRole] = useState<"User" | "Author">("User");
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError(null);
    setSubmitting(true);
    try {
      await register({ Email: email, FirstName: firstName, LastName: lastName, Password: password, Role: role });
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
        <h1 className="text-xl text-slate-800">Register</h1>
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
        <div className="flex gap-4">
          <label className="flex flex-1 flex-col gap-1 text-sm text-slate-600">
            First name
            <input
              type="text"
              value={firstName}
              onChange={(e) => setFirstName(e.target.value)}
              required
              className="rounded-sm border border-slate-300 px-3 py-2 text-slate-800 outline-none focus:border-slate-500"
            />
          </label>
          <label className="flex flex-1 flex-col gap-1 text-sm text-slate-600">
            Last name
            <input
              type="text"
              value={lastName}
              onChange={(e) => setLastName(e.target.value)}
              required
              className="rounded-sm border border-slate-300 px-3 py-2 text-slate-800 outline-none focus:border-slate-500"
            />
          </label>
        </div>
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
        <label className="flex flex-col gap-1 text-sm text-slate-600">
          Role
          <select
            value={role}
            onChange={(e) => setRole(e.target.value as "User" | "Author")}
            className="rounded-sm border border-slate-300 px-3 py-2 text-slate-800 outline-none focus:border-slate-500"
          >
            <option value="User">User</option>
            <option value="Author">Author</option>
          </select>
        </label>
        {error && <p className="text-sm text-red-600">{error}</p>}
        <button
          type="submit"
          disabled={submitting}
          className="rounded-sm bg-slate-800 py-2 text-sm text-white disabled:opacity-50"
        >
          {submitting ? "Registering…" : "Register"}
        </button>
        <p className="text-sm text-slate-600">
          Already have an account?{" "}
          <Link href="/login" className="text-slate-800 underline">
            Log in
          </Link>
        </p>
      </form>
    </main>
  );
}