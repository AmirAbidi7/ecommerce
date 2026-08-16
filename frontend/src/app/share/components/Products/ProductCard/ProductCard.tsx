"use client";

import Image from "next/image";
import { ProductOverview } from "../../../types";
import FavoriteButton from "./FavoriteButton";

export default function ProductCard({
  product,
  favorited,
  onToggleFavorite,
}: {
  product: ProductOverview;
  favorited: boolean;
  onToggleFavorite: (productId: string, favorite: boolean) => void;
}) {
  return (
    <div className="flex flex-col justify-center items-center rounded-sm">
      <FavoriteButton
        productId={product.Id}
        initial={favorited}
        onToggle={(next) => onToggleFavorite(product.Id, next)}
      />
      {product.ImageUrl ? (
        <Image
          src={product.ImageUrl}
          className="mx-4"
          alt={product.Name}
          width={200}
          height={200}
          unoptimized
        />
      ) : (
        <div className="mx-4 flex h-[200px] w-[200px] items-center justify-center bg-slate-100 text-sm text-slate-400">
          No image
        </div>
      )}
      <h3 className="text-xl text-slate-800">{product.Name}</h3>
      <p className="text-xs text-slate-400">{product.CategoryName}</p>
      {product.IsOnSale ? (
        <p className="text-left text-sm text-slate-600">
          <span className="line-through text-slate-400">{product.Price} TND</span>{" "}
          <span className="text-red-600 font-semibold">{product.EffectivePrice} TND</span>
          <span className="ml-1 badge text-xs bg-red-100 text-red-700 px-1 rounded">-{product.SalePercent}%</span>
        </p>
      ) : (
        <p className="text-left text-sm text-slate-600">{product.Price} TND</p>
      )}
    </div>
  );
}