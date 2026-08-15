"use client";

import { useState } from "react";
import { api } from "../../../api";
import { useAuth } from "../../../auth/AuthContext";
import { useRouter } from "next/navigation";

export default function FavoriteButton({ productId, initial }: { productId: string; initial: boolean }) {
  const { token } = useAuth();
  const router = useRouter();
  const [favorite, setFavorite] = useState(initial);
  const [busy, setBusy] = useState(false);

  const toggle = async () => {
    if (!token) {
      router.push("/login");
      return;
    }
    setBusy(true);
    try {
      setFavorite((f) => !f);
      await api<void>(favorite ? "/api/product/unfavorite" : "/api/product/favorite", {
        method: "PUT",
        body: productId,
      });
    } finally {
      setBusy(false);
    }
  };

  return (
    <button
      onClick={toggle}
      disabled={busy}
      className={favorite ? "text-red-600" : "text-slate-400"}
      aria-label={favorite ? "Unfavorite" : "Favorite"}
    >
      {favorite ? "♥" : "♡"}
    </button>
  );
}