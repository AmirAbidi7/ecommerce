export interface ProductOverview {
  Id: string;
  Name: string;
  Price: number;
  ImageUrl: string;
}

export interface ProductDetails extends ProductOverview {
  Description: string;
}

const PRODUCT_ENDPOINT = "http://localhost:8000/api/product";
export async function getProducts() {
  const response = await fetch(PRODUCT_ENDPOINT);

  if (response.status !== 200) {
    return "Error fetching products!";
  }

  const products: Array<ProductOverview> = await response.json();

  return products;
}

export async function getProduct(id: string) {
  const response = await fetch(PRODUCT_ENDPOINT + `/${id}`);

  if (response.status !== 200) {
    return "Error fetching this product!";
  }

  const product: ProductDetails = await response.json();

  return product;
}
