"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "../share/auth/AuthContext";
import AddBookForm from "./AddBookForm";
import MyBooks from "./MyBooks";
import SalesTable from "./SalesTable";

export default function AuthorPage() {
  const { user, loading } = useAuth();
  const router = useRouter();
  const [booksKey, setBooksKey] = useState(0);

  useEffect(() => {
    if (!loading && !user) router.push("/login");
  }, [loading, user, router]);

  if (loading || !user) {
    return (
      <main className="flex flex-1 items-center justify-center">
        <p className="text-slate-600">{loading ? "Loading…" : "Redirecting…"}</p>
      </main>
    );
  }

  return (
    <main className="flex flex-1 flex-col gap-8 p-8">
      <h1 className="text-2xl text-slate-800">Author dashboard</h1>
      <AddBookForm onCreated={() => setBooksKey((k) => k + 1)} />
      <MyBooks key={booksKey} />
      <SalesTable />
    </main>
  );
}