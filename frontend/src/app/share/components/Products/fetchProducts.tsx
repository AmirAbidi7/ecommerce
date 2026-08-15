import { api } from "../../api";
import { ProductDetails, ProductOverview } from "../../types";

const PRODUCT_ENDPOINT = "/api/product";

export async function getProducts(): Promise<ProductOverview[] | { error: string }> {
  try {
    return await api<ProductOverview[]>(PRODUCT_ENDPOINT);
  } catch (e) {
    return { error: "Error fetching products!" };
  }
}

export async function getProduct(id: string): Promise<ProductDetails | { error: string }> {
  try {
    return await api<ProductDetails>(`${PRODUCT_ENDPOINT}/${id}`);
  } catch {
    return { error: "Error fetching this product!" };
  }
}