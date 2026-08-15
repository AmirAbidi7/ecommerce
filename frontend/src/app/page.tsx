"use client";

import Link from "next/link";
import { useAuth } from "./share/auth/AuthContext";

export default function Home() {
  const { user } = useAuth();

  return (
    <main className="flex flex-1 flex-col items-center justify-center gap-6">
      <h1 className="text-3xl text-slate-800">Ecommerce</h1>
      <nav className="flex flex-col items-center gap-2 text-sm">
        <Link href="/products" className="text-slate-600 hover:text-slate-800">
          Products
        </Link>
        <Link href="/login" className="text-slate-600 hover:text-slate-800">
          Log in
        </Link>
        <Link href="/register" className="text-slate-600 hover:text-slate-800">
          Register
        </Link>
        {user && (
          <Link href="/author" className="text-slate-600 hover:text-slate-800">
            Author dashboard
          </Link>
        )}
      </nav>
    </main>
  );
}