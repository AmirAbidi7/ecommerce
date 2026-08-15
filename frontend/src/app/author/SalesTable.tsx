"use client";

import { useEffect, useState } from "react";
import { api, ApiError } from "../share/api";
import { AuthorSaleStat } from "../share/types";

export default function SalesTable() {
  const [sales, setSales] = useState<AuthorSaleStat[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    api<AuthorSaleStat[]>("/api/author/sales").then(
      (data) => {
        setSales(data);
        setLoading(false);
      },
      (err) => {
        setError(
          err instanceof ApiError && err.status === 403
            ? "Author privileges needed"
            : err instanceof Error
              ? err.message
              : String(err),
        );
        setLoading(false);
      },
    );
  }, []);

  if (loading) return <p className="text-sm text-slate-600">Loading sales…</p>;
  if (error)
    return <p className="text-sm text-red-600">Sales: {error}</p>;

  return (
    <section>
      <h2 className="text-lg text-slate-800">Sales</h2>
      {sales.length === 0 ? (
        <p className="text-sm text-slate-600">No sales yet</p>
      ) : (
        <table className="mt-2 w-full max-w-xl border-collapse text-sm">
          <thead>
            <tr className="border-b border-slate-200 text-left text-slate-600">
              <th className="py-2 pr-4 font-medium">Name</th>
              <th className="py-2 pr-4 font-medium">Units Sold</th>
              <th className="py-2 font-medium">Revenue (TND)</th>
            </tr>
          </thead>
          <tbody>
            {sales.map((s) => (
              <tr key={s.ProductId} className="border-b border-slate-200 text-slate-800">
                <td className="py-2 pr-4">{s.Name}</td>
                <td className="py-2 pr-4">{s.UnitsSold}</td>
                <td className="py-2">{s.Revenue.toFixed(2)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </section>
  );
}