// Models for checkout
export interface OrderCreateDto {
  userId: string;
  address: string; 
  fullName: string;
  contactNumber: string;
  orderItems: OrderItemCreateDto[];
  paymentDetail?: PaymentDetailCreateDto;
}

export interface OrderItemCreateDto {
  productId: string;
  cartItemId: string;
  productName: string;
  discountId?: string;
  basePrice: number;
  discountPercent?: number;
  productColorId: string;
  colorName: string;
  colorCode?: string;
  productVariationId: string;
  size: string;
  inventoryId: string;
  quantity: number;
  imageUrl?: string;
}

export interface PaymentDetailCreateDto {
  paymentMethod: string;
  amount: number;
  transactionId?: string;
}