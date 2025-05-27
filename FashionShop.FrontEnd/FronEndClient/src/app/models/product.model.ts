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