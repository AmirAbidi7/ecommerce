import Image from "next/image";
import { ProductOverview } from "../fetchProducts";

export default function ProductCard(product: ProductOverview) {
  return (
    <div className="flex flex-col justify-center items-center rounded-sm">
      <Image src={product.ImageUrl} className="mx-4" alt="product idk" />
      <h3 className="text-xl text-slate-800">{product.Name}</h3>

      <p className="text-left text-sm text-slate-600">{product.Price} TND</p>
    </div>
  );
}
