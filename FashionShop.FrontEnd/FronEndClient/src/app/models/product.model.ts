export interface Product {
  id: string;
  name: string;
  basePrice: number;
  discountedPrice?: number | null;
  averageRating?: number | null;
  totalRating?: number | null;
  mainImageUrl: string;
  discountDisplayDTO?: any | null;
}
export interface ProductDetails {
  id: string;
  name: string;
  description: string | null;
  basePrice: number;
  discountedPrice: number | null;
  mainImageUrl: string;
  discountDisplayDTO: DiscountDisplay | null;
  productColorsDisplayDTO: ProductColor[];
  productCategoryDisplayDTO: ProductCategory;
}

export interface DiscountDisplay {
  id: string;
  name: string;
  discountPercent: number;
  isActive: boolean;
}

export interface ProductColor {
  id: string;
  colorName: string;
  colorCode: string | null;
  imageUrlColor: string;
  productVariationDisplayDTOs: ProductVariation[];
}

export interface ProductVariation {
  id: string;
  size: string;
  description: string | null;
  quantity: number;
  imageUrlVariation: string | null;
}

export interface ProductCategory {
  id: string;
  name: string;
  imageUrl: string | null;
  subCategory: ProductCategory[] | null;
}