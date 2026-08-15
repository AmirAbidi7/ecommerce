"use client";

import { FormEvent, useEffect, useState } from "react";
import { api, ApiError } from "../share/api";
import { AuthorBook } from "../share/types";

function SaleForm({
  bookId,
  onDone,
}: {
  bookId: string;
  onDone: () => void;
}) {
  const [percent, setPercent] = useState("");
  const [starts, setStarts] = useState("");
  const [ends, setEnds] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const percentNum = Number(percent);
  const startMs = starts ? Date.parse(starts) : NaN;
  const endMs = ends ? Date.parse(ends) : NaN;
  const missing = !percent || !starts || !ends;
  const invalid = percentNum < 1 || percentNum > 100;
  const ordering = starts && ends && endMs <= startMs;
  const formInvalid = missing || invalid || ordering;

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    setError(null);
    try {
      await api<void>(`/api/product/${bookId}/sale`, {
        method: "POST",
        body: {
          PercentOff: percentNum,
          StartsAt: new Date(starts).toISOString(),
          EndsAt: new Date(ends).toISOString(),
        },
      });
      setPercent("");
      setStarts("");
      setEnds("");
      onDone();
    } catch (err) {
      setError(
        err instanceof ApiError && err.status === 403
          ? "Author privileges needed"
          : err instanceof Error
            ? err.message
            : String(err),
      );
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <form onSubmit={onSubmit} className="mt-3 flex flex-col gap-3 border-t border-slate-200 pt-3">
      <div className="flex flex-wrap gap-3 text-sm">
        <label className="flex flex-col gap-1 text-slate-600">
          Percent
          <input
            type="number"
            min="1"
            max="100"
            value={percent}
            onChange={(e) => setPercent(e.target.value)}
            required
            className="w-24 rounded-sm border border-slate-300 px-2 py-1 text-slate-800 outline-none focus:border-slate-500"
          />
        </label>
        <label className="flex flex-col gap-1 text-slate-600">
          Starts
          <input
            type="datetime-local"
            value={starts}
            onChange={(e) => setStarts(e.target.value)}
            required
            className="rounded-sm border border-slate-300 px-2 py-1 text-slate-800 outline-none focus:border-slate-500"
          />
        </label>
        <label className="flex flex-col gap-1 text-slate-600">
          Ends
          <input
            type="datetime-local"
            value={ends}
            onChange={(e) => setEnds(e.target.value)}
            required
            className="rounded-sm border border-slate-300 px-2 py-1 text-slate-800 outline-none focus:border-slate-500"
          />
        </label>
      </div>
      {!missing && (invalid || ordering) && (
        <p className="text-sm text-red-600">
          {ordering
            ? "Sale must end after it starts"
            : "Percent must be between 1 and 100"}
        </p>
      )}
      {error && <p className="text-sm text-red-600">{error}</p>}
      <button
        type="submit"
        disabled={formInvalid || submitting}
        className="w-fit rounded-sm bg-slate-800 px-4 py-1 text-sm text-white disabled:opacity-50"
      >
        {submitting ? "Creating…" : "Start sale"}
      </button>
    </form>
  );
}

export default function MyBooks() {
  const [books, setBooks] = useState<AuthorBook[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [busyId, setBusyId] = useState<string | null>(null);
  const [actionError, setActionError] = useState<string | null>(null);

  const load = () => {
    api<AuthorBook[]>("/api/author/products").then(
      (data) => {
        setBooks(data);
        setError(null);
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
  };

  useEffect(() => {
    load();
  }, []);

  const runAction = async (id: string, fn: () => Promise<void>) => {
    setBusyId(id);
    setActionError(null);
    try {
      await fn();
      await load();
    } catch (err) {
      setActionError(
        err instanceof ApiError && err.status === 403
          ? "Author privileges needed"
          : err instanceof Error
            ? err.message
            : String(err),
      );
    } finally {
      setBusyId(null);
    }
  };

  if (loading) return <p className="text-sm text-slate-600">Loading books…</p>;
  if (error) return <p className="text-sm text-red-600">Books: {error}</p>;

  return (
    <section>
      <h2 className="text-lg text-slate-800">My books</h2>
      {actionError && (
        <p className="mt-2 text-sm text-red-600">{actionError}</p>
      )}
      {books.length === 0 ? (
        <p className="text-sm text-slate-600">
          No books yet — add your first book above.
        </p>
      ) : (
        <ul className="mt-2 flex flex-col gap-4">
          {books.map((book) => (
            <li
              key={book.Id}
              className="rounded-sm border border-slate-200 p-4"
            >
              <div className="flex items-baseline justify-between gap-4">
                <div>
                  <p className="text-slate-800">
                    {book.Name}{" "}
                    <span className="text-sm text-slate-600">
                      ({book.CategoryName})
                    </span>
                  </p>
                  <p className="text-sm text-slate-600">
                    {book.IsOnSale ? (
                      <>
                        <span className="line-through">{book.Price.toFixed(2)}</span>{" "}
                        <span className="text-slate-800">
                          {book.EffectivePrice.toFixed(2)} TND
                        </span>
                      </>
                    ) : (
                      <span>{book.Price.toFixed(2)} TND</span>
                    )}{" "}
                    · Stock: {book.Stock}
                    {!book.IsListed && (
                      <span className="ml-2 rounded-sm bg-slate-200 px-2 py-0.5 text-xs text-slate-600">
                        Unlisted
                      </span>
                    )}
                  </p>
                </div>
                {book.IsListed && (
                  <button
                    onClick={() =>
                      runAction(book.Id, () =>
                        api<void>(`/api/product/${book.Id}`, {
                          method: "DELETE",
                        }),
                      )
                    }
                    disabled={busyId === book.Id}
                    className="rounded-sm border border-slate-300 px-3 py-1 text-sm text-slate-600 disabled:opacity-50"
                  >
                    Unlist
                  </button>
                )}
              </div>

              {book.IsOnSale && book.SalePercent != null ? (
                <div className="mt-3 flex items-center justify-between gap-4 border-t border-slate-200 pt-3 text-sm text-slate-600">
                  <p>
                    On sale:{" "}
                    <span className="font-medium text-slate-800">
                      -{book.SalePercent}% off
                    </span>
                  </p>
                  <button
                    onClick={() =>
                      runAction(book.Id, () =>
                        api<void>(`/api/product/${book.Id}/sale`, {
                          method: "DELETE",
                        }),
                      )
                    }
                    disabled={busyId === book.Id}
                    className="rounded-sm border border-slate-300 px-3 py-1 text-sm text-slate-600 disabled:opacity-50"
                  >
                    Cancel sale
                  </button>
                </div>
              ) : (
                <SaleForm
                  bookId={book.Id}
                  onDone={() => runAction(book.Id, () => Promise.resolve())}
                />
              )}
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}