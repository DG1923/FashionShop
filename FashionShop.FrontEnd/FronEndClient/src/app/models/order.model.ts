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

export enum OrderStatus {
  Pending = 0,
  Confirmed = 1,
  Shipping = 2,
  Delivered = 3,
  ReturnRequested = 4,
  ReturnApproved = 5,
  ReturnRejected = 6,
  Completed = 7,
  Cancelled = 8
}

export interface Order {
  id: string;
  userId: string;
  address: string;
  status: OrderStatus;
  total: number;
  fullName: string;
  contactNumber: string;
  createdAt: Date;
  returnReason?: string; // Add this field
}