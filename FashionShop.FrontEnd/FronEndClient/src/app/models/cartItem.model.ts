export interface CartItem {
  cartId: string;
  productId: string;
  productName: string;
  basePrice: number;
  discountPercent: number;
  colorName: string;
  colorCode: string| null;
  size: string;
  quantity: number;
  imageUrl: string;
  inventoryId: string;
  productColorId: string;
  productVariationId: string;
}