"use client";

import { useEffect, useMemo, useState } from "react";
import ProductCard from "../share/components/Products/ProductCard/ProductCard";
import { getProducts } from "../share/components/Products/fetchProducts";
import { ProductOverview } from "../share/types";

export default function Products() {
  const [products, setProducts] = useState<ProductOverview[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [category, setCategory] = useState<string>("");

  useEffect(() => {
    getProducts().then((res) => {
      if ("error" in res) setError(res.error);
      else setProducts(res);
    });
  }, []);

  const categories = useMemo(
    () => [...new Set(products.map((p) => p.CategoryName))].sort(),
    [products],
  );

  const visible = category
    ? products.filter((p) => p.CategoryName === category)
    : products;

  return (
    <div className="p-8">
      {error && <p className="text-red-600">{error}</p>}
      <label className="block mb-4">
        Category:{" "}
        <select value={category} onChange={(e) => setCategory(e.target.value)}>
          <option value="">All</option>
          {categories.map((c) => (
            <option key={c} value={c}>{c}</option>
          ))}
        </select>
      </label>
      <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
        {visible.map((p) => (
          <ProductCard key={p.Id} {...p} />
        ))}
      </div>
    </div>
  );
}