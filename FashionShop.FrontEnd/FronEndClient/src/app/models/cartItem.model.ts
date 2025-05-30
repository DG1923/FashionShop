export interface CartItem {
  id: string;
  cartId: string;
  productId: string;
  productName: string;
  basePrice: number;
  discountPercent: number;
  colorName: string;
  colorCode: string;
  size: string;
  quantity: number;
  imageUrl: string;
  inventoryId: string;
  productColorId: string;
  productVariationId: string;
}