import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CartService } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';

interface CartItem {
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

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './cart.component.html'
})
export class CartComponent implements OnInit {
  cartItems: CartItem[] = [];
  isLoading = false;
  error: string | null = null;

  constructor(
    private cartService: CartService,
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.loadCart();
  }

  loadCart() {
    this.isLoading = true;
    const userId = this.authService.getUserId();
    
    if (!userId) {
      this.error = 'Vui lòng đăng nhập để xem giỏ hàng';
      this.isLoading = false;
      return;
    }

    // First get cartId by userId
    this.cartService.getCartIdByUserId(userId).subscribe({
      next: (cartId) => {
        // Then get cart items using cartId
        this.cartService.getCartByUserId(cartId).subscribe({
          next: (cartItems) => {
            this.cartItems = cartItems;
            this.isLoading = false;
          },
          error: (error) => {
            console.error('Error loading cart items:', error);
            this.error = 'Không thể tải giỏ hàng';
            this.isLoading = false;
          }
        });
      },
      error: (error) => {
        console.error('Error getting cart ID:', error);
        this.error = 'Không thể tìm thấy giỏ hàng';
        this.isLoading = false;
      }
    });
  }

  updateQuantity(item: CartItem, newQuantity: number) {
    if (newQuantity < 1) return;
    
    const updateDto = {
      id: item.id,
      cartId: item.cartId,
      productId: item.productId,
      productColorId: item.productColorId,
      colorName: item.colorName,
      colorCode: item.colorCode,
      productVariationId: item.productVariationId,
      size: item.size,
      inventoryId: item.inventoryId,
      quantity: newQuantity
    };

    this.cartService.updateCartItem(item.id, updateDto).subscribe({
      next: () => {
        item.quantity = newQuantity;
      },
      error: (error) => {
        console.error('Error updating quantity:', error);
      }
    });
  }

  removeItem(itemId: string) {
    this.cartService.removeCartItem(itemId).subscribe({
      next: () => {
        this.cartItems = this.cartItems.filter(item => item.id !== itemId);
      },
      error: (error) => {
        console.error('Error removing item:', error);
      }
    });
  }

  getSubtotal(item: CartItem): number {
    return item.basePrice * item.quantity * (1 - item.discountPercent/100);
  }

  getTotal(): number {
    return this.cartItems.reduce((sum, item) => sum + this.getSubtotal(item), 0);
  }

  incrementQuantity(item: CartItem) {
    this.updateQuantity(item, item.quantity + 1);
  }

  decrementQuantity(item: CartItem) {
    if (item.quantity > 1) {
      this.updateQuantity(item, item.quantity - 1);
    }
  }
}
