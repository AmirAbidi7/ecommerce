"use client";

import { FormEvent, useState } from "react";
import { api, ApiError } from "../share/api";
import { CreateProductRequest, ProductOverview } from "../share/types";

const emptyForm: CreateProductRequest = {
  Name: "",
  Price: 0,
  Stock: 0,
  ImageUrl: "",
  Description: "",
  CategoryName: "",
};

export default function AddBookForm({ onCreated }: { onCreated: () => void }) {
  const [form, setForm] = useState<CreateProductRequest>(emptyForm);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const setField = (field: keyof CreateProductRequest, value: string) => {
    setForm((f) => ({ ...f, [field]: value }));
  };

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    setError(null);
    try {
      await api<ProductOverview>("/api/product", {
        method: "POST",
        body: {
          ...form,
          Name: form.Name.trim(),
          Price: Number(form.Price),
          Stock: Number(form.Stock),
          ImageUrl: form.ImageUrl.trim(),
          Description: form.Description.trim(),
          CategoryName: form.CategoryName.trim(),
        },
      });
      setForm(emptyForm);
      onCreated();
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
    <form
      onSubmit={onSubmit}
      className="flex w-full max-w-md flex-col gap-4 rounded-sm border border-slate-200 p-6"
    >
      <h2 className="text-lg text-slate-800">Add a book</h2>
      <label className="flex flex-col gap-1 text-sm text-slate-600">
        Name
        <input
          type="text"
          value={form.Name}
          onChange={(e) => setField("Name", e.target.value)}
          required
          className="rounded-sm border border-slate-300 px-3 py-2 text-slate-800 outline-none focus:border-slate-500"
        />
      </label>
      <div className="grid grid-cols-2 gap-4">
        <label className="flex flex-col gap-1 text-sm text-slate-600">
          Price
          <input
            type="number"
            step="0.01"
            min="0"
            value={form.Price}
            onChange={(e) => setField("Price", e.target.value)}
            required
            className="rounded-sm border border-slate-300 px-3 py-2 text-slate-800 outline-none focus:border-slate-500"
          />
        </label>
        <label className="flex flex-col gap-1 text-sm text-slate-600">
          Stock
          <input
            type="number"
            min="0"
            value={form.Stock}
            onChange={(e) => setField("Stock", e.target.value)}
            required
            className="rounded-sm border border-slate-300 px-3 py-2 text-slate-800 outline-none focus:border-slate-500"
          />
        </label>
      </div>
      <label className="flex flex-col gap-1 text-sm text-slate-600">
        Image URL
        <input
          type="text"
          value={form.ImageUrl}
          onChange={(e) => setField("ImageUrl", e.target.value)}
          className="rounded-sm border border-slate-300 px-3 py-2 text-slate-800 outline-none focus:border-slate-500"
        />
      </label>
      <label className="flex flex-col gap-1 text-sm text-slate-600">
        Description
        <input
          type="text"
          value={form.Description}
          onChange={(e) => setField("Description", e.target.value)}
          className="rounded-sm border border-slate-300 px-3 py-2 text-slate-800 outline-none focus:border-slate-500"
        />
      </label>
      <label className="flex flex-col gap-1 text-sm text-slate-600">
        Category
        <input
          type="text"
          value={form.CategoryName}
          onChange={(e) => setField("CategoryName", e.target.value)}
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
        {submitting ? "Adding…" : "Add book"}
      </button>
    </form>
  );
}