export interface CartItemAddDto {
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

export interface CartItem {
  id: string;
  cartId: string;
  productId: string;
  productName: string;
  discountId: string | null;
  basePrice: number;
  discountPercent: number;
  productColorId: string;
  colorName: string;
  colorCode: string | null;
  productVariationId: string;
  size: string;
  inventoryId: string;
  quantity: number;
  imageUrl: string;
  totalPrice: number;
}
export interface CartItemUpdateDto {
  id: string; // ID của cart item
  cartId: string; // ID của giỏ hàng
  productId: string; // ID sản phẩm
  productColorId: string; // ID màu sản phẩm
  productVariationId: string; // ID variation (size)
  inventoryId: string; // ID tồn kho
  colorName: string; // Tên màu
  colorCode: string | null; 
  size: string; 
  quantity: number; 
}