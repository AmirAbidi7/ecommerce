export interface UserInfo {
  Id: string;
  Email: string;
  FirstName: string;
  LastName: string;
}

export interface AuthResult {
  IsSuccess: boolean;
  UserInfo: UserInfo | null;
  Token: string | null;
  RefreshToken: string | null;
}

export interface ProductOverview {
  Id: string;
  Name: string;
  Price: number;
  ImageUrl: string;
  CategoryName: string;
  IsOnSale: boolean;
  SalePercent: number | null;
  EffectivePrice: number;
}

export interface ProductDetails extends ProductOverview {
  Description: string;
}

export interface AuthorBook {
  Id: string;
  Name: string;
  Price: number;
  EffectivePrice: number;
  IsOnSale: boolean;
  SalePercent: number | null;
  IsListed: boolean;
  Stock: number;
  CategoryName: string;
}

export interface AuthorSaleStat {
  ProductId: string;
  Name: string;
  UnitsSold: number;
  Revenue: number;
}

// Request bodies MUST stay PascalCase — backend binding is case-sensitive.
export interface CreateProductRequest {
  Name: string;
  Price: number;
  Stock: number;
  ImageUrl: string;
  Description: string;
  CategoryName: string;
}

export interface CreateSaleRequest {
  PercentOff: number;
  StartsAt: string;
  EndsAt: string;
}