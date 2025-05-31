import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface Toast {
  id: number;
  message: string;
  type: 'success' | 'error' | 'warning' | 'info';
  duration?: number;
  dismissible?: boolean;
  icon?: string;
  action?: {
    label: string;
    handler: () => void;
  };
  isVisible?: boolean;
  isRemoving?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  private toastSubject = new BehaviorSubject<Toast[]>([]);
  toasts$ = this.toastSubject.asObservable();

  success(message: string, options?: Partial<Toast>) {
    this.show(message, 'success', {
      icon: '✓',
      duration: 4000,
      ...options
    });
  }

  error(message: string, options?: Partial<Toast>) {
    this.show(message, 'error', {
      icon: '✕',
      duration: 5000,
      dismissible: true,
      ...options
    });
  }

  warning(message: string, options?: Partial<Toast>) {
    this.show(message, 'warning', {
      icon: '⚠',
      duration: 4500,
      ...options
    });
  }

  info(message: string, options?: Partial<Toast>) {
    this.show(message, 'info', {
      icon: 'ℹ',
      duration: 4000,
      ...options
    });
  }

  // Special toasts for fashion app
  cartSuccess(message: string = 'Đã thêm vào giỏ hàng') {
    this.show(message, 'success', {
      icon: '🛍️',
      duration: 3000,
      action: {
        label: 'Xem giỏ hàng',
        handler: () => {
          // Navigate to cart - you can inject Router here if needed
          console.log('Navigate to cart');
        }
      }
    });
  }

  wishlistSuccess(message: string = 'Đã thêm vào danh sách yêu thích') {
    this.show(message, 'success', {
      icon: '❤️',
      duration: 3000
    });
  }

  orderSuccess(message: string = 'Đặt hàng thành công') {
    this.show(message, 'success', {
      icon: '🎉',
      duration: 5000,
      action: {
        label: 'Xem đơn hàng',
        handler: () => {
          console.log('Navigate to orders');
        }
      }
    });
  }

  private show(message: string, type: Toast['type'], options: Partial<Toast> = {}) {
    const toast: Toast = {
      id: Date.now() + Math.random(),
      message,
      type,
      duration: 4000,
      dismissible: true,
      isVisible: false,
      isRemoving: false,
      ...options
    };

    const currentToasts = this.toastSubject.value;
    this.toastSubject.next([...currentToasts, toast]);

    // Trigger entrance animation
    setTimeout(() => {
      this.updateToast(toast.id, { isVisible: true });
    }, 50);

    // Auto remove after specified duration
    if (toast.duration && toast.duration > 0) {
      setTimeout(() => {
        this.dismiss(toast.id);
      }, toast.duration);
    }
  }

  dismiss(id: number) {
    // Start exit animation
    this.updateToast(id, { isRemoving: true });
    
    // Remove after animation completes
    setTimeout(() => {
      const toasts = this.toastSubject.value;
      this.toastSubject.next(toasts.filter(t => t.id !== id));
    }, 350); // Match animation duration
  }

  dismissAll() {
    const toasts = this.toastSubject.value;
    toasts.forEach(toast => {
      if (!toast.isRemoving) {
        this.dismiss(toast.id);
      }
    });
  }

  private updateToast(id: number, updates: Partial<Toast>) {
    const toasts = this.toastSubject.value;
    const updatedToasts = toasts.map(toast => 
      toast.id === id ? { ...toast, ...updates } : toast
    );
    this.toastSubject.next(updatedToasts);
  }

  // Get toast count for UI purposes
  getActiveCount(): number {
    return this.toastSubject.value.length;
  }

  // Clear all toasts immediately (useful for page navigation)
  clear() {
    this.toastSubject.next([]);
  }
}