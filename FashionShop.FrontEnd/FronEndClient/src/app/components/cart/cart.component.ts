import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CartService } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';
import { CartItem, CartItemAddDto, CartItemUpdateDto } from '../../models/cartItem.model';

interface CartItemState extends CartItem {
  isUpdating?: boolean;
  isRemoving?: boolean;
  hasError?: boolean;
  errorMessage?: string;
}

interface NotificationMessage {
  id: string;
  type: 'success' | 'error' | 'warning';
  message: string;
  show: boolean;
}

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {
  cartId: string = '';
  cartItems: CartItemAddDto[] = [];
  cartItemsDisplay: CartItemState[] = [];
  isLoading = false;
  error: string | null = null;
  notifications: NotificationMessage[] = [];

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
            this.cartItemsDisplay = cartItems.map(item => ({
              ...item,
              isUpdating: false,
              isRemoving: false,
              hasError: false,
              errorMessage: ''
            }));
            this.cartId = cartId;
            this.isLoading = false;
          },
          error: (error) => {
            console.error('Error loading cart items:', error);
            this.error = 'Không thể tải giỏ hàng';
            this.isLoading = false;
            this.showNotification('error', 'Có lỗi xảy ra khi tải giỏ hàng. Vui lòng thử lại.');
          }
        });
      },
      error: (error) => {
        console.error('Error getting cart ID:', error);
        this.error = 'Không thể tìm thấy giỏ hàng';
        this.isLoading = false;
        this.showNotification('error', 'Không thể tìm thấy giỏ hàng của bạn.');
      }
    });
  }

  updateQuantity(item: CartItemState, newQuantity: number) {
    // Validation checks
    if (newQuantity < 1) {
      this.showNotification('warning', 'Số lượng phải lớn hơn 0');
      return;
    }
    
    if (item.quantity === newQuantity) return; // No change in quantity
    
    if (item.quantity < newQuantity && item.inventoryId === null) {
      this.showNotification('warning', 'Sản phẩm này không có sẵn trong kho, vui lòng chọn số lượng khác.');
      return;
    }

    // Set loading state for this item
    item.isUpdating = true;
    item.hasError = false;
    
    const originalQuantity = item.quantity;
    const updateDto: CartItemUpdateDto = {
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
        // Animate the update
        setTimeout(() => {
          item.quantity = newQuantity;
          item.totalPrice = this.getSubtotal(item);
          item.isUpdating = false;
          this.cartService.updateCartCount();
          this.showNotification('success', 'Đã cập nhật số lượng sản phẩm');
        }, 300);
      },
      error: (error) => {
        console.error('Error updating quantity:', error);
        item.isUpdating = false;
        item.hasError = true;
        item.errorMessage = 'Không thể cập nhật số lượng';
        item.quantity = originalQuantity; // Revert to original quantity
        
        // Handle specific error cases
        if (error.status === 400) {
          this.showNotification('error', 'Số lượng yêu cầu vượt quá tồn kho');
        } else if (error.status === 404) {
          this.showNotification('error', 'Sản phẩm không tồn tại');
        } else {
          this.showNotification('error', 'Có lỗi xảy ra khi cập nhật. Vui lòng thử lại.');
        }

        // Clear error after 3 seconds
        setTimeout(() => {
          item.hasError = false;
          item.errorMessage = '';
        }, 3000);
      }
    });
  }

  removeItem(itemId: string) {
    const item = this.cartItemsDisplay.find(i => i.id === itemId);
    if (!item) return;

    if (!confirm('Bạn có chắc chắn muốn xóa sản phẩm này khỏi giỏ hàng?')) {
      return;
    }

    // Set removing state
    item.isRemoving = true;
    item.hasError = false;

    this.cartService.removeCartItem(itemId).subscribe({
      next: () => {
        // Animate removal
        setTimeout(() => {
          this.cartItemsDisplay = this.cartItemsDisplay.filter(i => i.id !== itemId);
          this.cartService.updateCartCount();
          this.showNotification('success', 'Đã xóa sản phẩm khỏi giỏ hàng');
        }, 300);
      },
      error: (error) => {
        console.error('Error removing item:', error);
        item.isRemoving = false;
        item.hasError = true;
        item.errorMessage = 'Không thể xóa sản phẩm';
        
        if (error.status === 404) {
          this.showNotification('error', 'Sản phẩm không tồn tại trong giỏ hàng');
        } else {
          this.showNotification('error', 'Có lỗi xảy ra khi xóa sản phẩm. Vui lòng thử lại.');
        }

        // Clear error after 3 seconds
        setTimeout(() => {
          item.hasError = false;
          item.errorMessage = '';
        }, 3000);
      }
    });
  }

  getSubtotal(item: CartItem): number {
    return item.basePrice * item.quantity * (1 - item.discountPercent/100);
  }

  getTotal(): number {
    let total: number = 0;
    this.cartItemsDisplay.forEach(item => {
      total += this.getSubtotal(item);
    });
    return total;
  }

  incrementQuantity(item: CartItemState) {
    if (item.isUpdating) return; // Prevent multiple clicks
    this.updateQuantity(item, item.quantity + 1);
  }

  decrementQuantity(item: CartItemState) {
    if (item.isUpdating) return; // Prevent multiple clicks
    if (item.quantity > 1) {
      this.updateQuantity(item, item.quantity - 1);
    }
  }

  // Notification system
  showNotification(type: 'success' | 'error' | 'warning', message: string) {
    const id = Date.now().toString();
    const notification: NotificationMessage = {
      id,
      type,
      message,
      show: true
    };

    this.notifications.push(notification);

    // Auto remove after 4 seconds
    setTimeout(() => {
      this.hideNotification(id);
    }, 4000);
  }

  hideNotification(id: string) {
    const notification = this.notifications.find(n => n.id === id);
    if (notification) {
      notification.show = false;
      // Remove from array after animation
      setTimeout(() => {
        this.notifications = this.notifications.filter(n => n.id !== id);
      }, 300);
    }
  }

  // Method to handle input blur for quantity validation
  onQuantityBlur(item: CartItemState, event: any) {
    const newQuantity = parseInt(event.target.value);
    if (isNaN(newQuantity) || newQuantity < 1) {
      event.target.value = item.quantity; // Reset to current value
      this.showNotification('warning', 'Vui lòng nhập số lượng hợp lệ');
      return;
    }
    this.updateQuantity(item, newQuantity);
  }

  // Retry failed operations
  retryOperation(item: CartItemState) {
    if (item.hasError) {
      item.hasError = false;
      item.errorMessage = '';
      // You could implement specific retry logic here
      this.showNotification('success', 'Đang thử lại...');
    }
  }
}