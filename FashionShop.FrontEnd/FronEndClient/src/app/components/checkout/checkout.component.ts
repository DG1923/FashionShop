import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Validators } from '@angular/forms';
import { OrderService } from '../../services/order.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastService } from '../../services/toast.service';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';
import { CommonModule } from '@angular/common';
import { OrderCreateDto, OrderItemCreateDto } from '../../models/order.model';
import { trigger, transition, style, animate, stagger, query } from '@angular/animations';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.css'],
  animations: [
    // Fade in animation for main container
    trigger('fadeInUp', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(30px)' }),
        animate('600ms cubic-bezier(0.35, 0, 0.25, 1)', 
          style({ opacity: 1, transform: 'translateY(0)' }))
      ])
    ]),
    
    // Stagger animation for form sections
    trigger('staggerFade', [
      transition(':enter', [
        query('.form-section', [
          style({ opacity: 0, transform: 'translateY(20px)' }),
          stagger(150, [
            animate('500ms cubic-bezier(0.35, 0, 0.25, 1)',
              style({ opacity: 1, transform: 'translateY(0)' }))
          ])
        ], { optional: true })
      ])
    ]),
    
    // Payment method selection animation
    trigger('paymentSelect', [
      transition(':enter', [
        style({ opacity: 0, transform: 'scale(0.95)' }),
        animate('300ms cubic-bezier(0.35, 0, 0.25, 1)',
          style({ opacity: 1, transform: 'scale(1)' }))
      ])
    ]),
    
    // Button hover and loading states
    trigger('buttonState', [
      transition('normal => loading', [
        animate('200ms ease-in-out')
      ]),
      transition('loading => normal', [
        animate('200ms ease-in-out')
      ])
    ]),
    
    // Error message animation
    trigger('errorSlide', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateX(-10px)', height: 0 }),
        animate('300ms cubic-bezier(0.35, 0, 0.25, 1)',
          style({ opacity: 1, transform: 'translateX(0)', height: '*' }))
      ]),
      transition(':leave', [
        animate('200ms ease-in-out',
          style({ opacity: 0, transform: 'translateX(-10px)', height: 0 }))
      ])
    ])
  ]
})
export class CheckoutComponent implements OnInit {
  checkoutForm!: FormGroup;
  isSubmitting = false;
  isLoading = true;
  cartItems: any[] = [];
  cartId: string = '';
  total: number = 0;
  shippingFee: number = 30000;
  
  // Error handling states
  formErrors: { [key: string]: string } = {};
  networkError: string = '';
  cartLoadError: boolean = false;

  paymentMethods = [
    { 
      id: 'COD', 
      name: 'Thanh toán khi nhận hàng', 
      icon: '💵',
      description: 'Thanh toán tiền mặt khi nhận hàng'
    },
    { 
      id: 'MOMO', 
      name: 'Ví MoMo', 
      icon: '📱',
      description: 'Thanh toán qua ví điện tử MoMo'
    },
    { 
      id: 'VNPAY', 
      name: 'VNPay', 
      icon: '💳',
      description: 'Thanh toán qua cổng VNPay'
    }
  ];

  constructor(
    private fb: FormBuilder,
    private orderService: OrderService,
    private router: Router,
    private notificationService: ToastService,
    private authService: AuthService,
    private cartService: CartService,
    private route: ActivatedRoute,
  ) {}

  ngOnInit() {
    this.initializeForm();
    this.loadInitialData();
  }

  private initializeForm() {
    this.checkoutForm = this.fb.group({
      fullName: ['', [
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(50)
      ]],
      contactNumber: ['', [
        Validators.required, 
        Validators.pattern('^(0[3|5|7|8|9])+([0-9]{8})$')
      ]],
      address: ['', [
        Validators.required,
        Validators.minLength(10),
        Validators.maxLength(200)
      ]],
      paymentMethod: ['COD', [Validators.required]]
    });

    // Real-time validation
    this.checkoutForm.valueChanges.subscribe(() => {
      this.updateFormErrors();
    });
  }

  private loadInitialData() {
    this.route.queryParams.subscribe(params => {
      this.cartId = params['cartId'];
      if (!this.cartId) {
        this.handleNavigationError('Không tìm thấy giỏ hàng');
        return;
      }
      this.loadCart();
    });
  }

  private handleNavigationError(message: string) {
    this.notificationService.error(message);
    setTimeout(() => {
      this.router.navigate(['/cart']);
    }, 2000);
  }

  loadCart() {
    this.isLoading = true;
    this.cartLoadError = false;
    this.networkError = '';

    const userId = this.authService.getUserId();
    if (!userId) {
      this.handleAuthError();
      return;
    }

    this.cartService.getCartIdByUserId(userId).subscribe({
      next: (cartId) => {
        this.cartId = cartId;
        this.loadCartItems(cartId);
      },
      error: (error) => {
        this.handleCartLoadError(error, 'Không thể tìm thấy giỏ hàng');
      }
    });
  }

  private loadCartItems(cartId: string) {
    this.cartService.getCartByUserId(cartId).subscribe({
      next: (items) => {
        this.cartItems = items || [];
        this.calculateTotal();
        this.isLoading = false;
        
        if (this.cartItems.length === 0) {
          this.handleEmptyCart();
        }
      },
      error: (error) => {
        this.handleCartLoadError(error, 'Không thể tải giỏ hàng');
      }
    });
  }

  private handleAuthError() {
    this.notificationService.warning('Vui lòng đăng nhập để tiếp tục');
    setTimeout(() => {
      this.router.navigate(['/login']);
    }, 2000);
  }

  private handleCartLoadError(error: any, message: string) {
    console.error('Cart load error:', error);
    this.isLoading = false;
    this.cartLoadError = true;
    
    if (error.status === 0) {
      this.networkError = 'Lỗi kết nối mạng. Vui lòng kiểm tra kết nối internet.';
    } else if (error.status === 404) {
      this.networkError = 'Giỏ hàng không tồn tại.';
    } else if (error.status >= 500) {
      this.networkError = 'Lỗi server. Vui lòng thử lại sau.';
    } else {
      this.networkError = message;
    }
    
    this.notificationService.error(this.networkError);
  }

  private handleEmptyCart() {
    this.notificationService.warning('Giỏ hàng trống');
    setTimeout(() => {
      this.router.navigate(['/products']);
    }, 2000);
  }

  calculateTotal(): number {
    this.total = this.cartItems.reduce((total, item) => {
      const price = item.discountPercent ? 
        item.basePrice * (1 - item.discountPercent / 100) : 
        item.basePrice;
      return total + (price * item.quantity);
    }, 0);
    return this.total;
  }

  getFinalTotal(): number {
    return this.total;
  }

  private updateFormErrors() {
    this.formErrors = {};
    Object.keys(this.checkoutForm.controls).forEach(key => {
      const control = this.checkoutForm.get(key);
      if (control && control.errors && control.touched) {
        this.formErrors[key] = this.getErrorMessage(key, control.errors);
      }
    });
  }

  private getErrorMessage(fieldName: string, errors: any): string {
    const fieldDisplayName = this.getFieldDisplayName(fieldName);
    
    if (errors['required']) {
      return `Vui lòng nhập ${fieldDisplayName}`;
    }
    if (errors['minlength']) {
      return `${fieldDisplayName} phải có ít nhất ${errors['minlength'].requiredLength} ký tự`;
    }
    if (errors['maxlength']) {
      return `${fieldDisplayName} không được vượt quá ${errors['maxlength'].requiredLength} ký tự`;
    }
    if (errors['pattern']) {
      if (fieldName === 'contactNumber') {
        return 'Số điện thoại không đúng định dạng (VD: 0912345678)';
      }
    }
    return `${fieldDisplayName} không hợp lệ`;
  }

  private getFieldDisplayName(fieldName: string): string {
    const displayNames: { [key: string]: string } = {
      'fullName': 'họ và tên',
      'contactNumber': 'số điện thoại', 
      'address': 'địa chỉ',
      'paymentMethod': 'phương thức thanh toán'
    };
    return displayNames[fieldName] || fieldName;
  }

  retryLoadCart() {
    this.loadCart();
  }

  onSubmit() {
    if (!this.validateForm()) {
      return;
    }

    if (this.cartItems.length === 0) {
      this.notificationService.warning('Giỏ hàng trống. Vui lòng thêm sản phẩm.');
      return;
    }

    this.processOrder();
  }

  private validateForm(): boolean {
    this.checkoutForm.markAllAsTouched();
    this.updateFormErrors();
    
    if (!this.checkoutForm.valid) {
      this.notificationService.warning('Vui lòng điền đầy đủ thông tin hợp lệ!');
      this.scrollToFirstError();
      return false;
    }
    
    return true;
  }

  private scrollToFirstError() {
    setTimeout(() => {
      const firstErrorElement = document.querySelector('.text-red-500');
      if (firstErrorElement) {
        firstErrorElement.scrollIntoView({ 
          behavior: 'smooth', 
          block: 'center' 
        });
      }
    }, 100);
  }

  private processOrder() {
    this.isSubmitting = true;
    this.networkError = '';

    const userId = this.authService.getUserId();
    if (!userId) {
      this.handleAuthError();
      return;
    }

    const orderData: OrderCreateDto = this.buildOrderData(userId);
    
    this.orderService.createOrder(this.cartId, orderData).subscribe({
      next: (response) => {
        this.handleOrderSuccess(response);
      },
      error: (error) => {
        this.handleOrderError(error);
      }
    });
  }

  private buildOrderData(userId: string): OrderCreateDto {
    return {
      userId: userId,
      fullName: this.checkoutForm.get('fullName')?.value.trim(),
      contactNumber: this.checkoutForm.get('contactNumber')?.value.trim(),
      address: this.checkoutForm.get('address')?.value.trim(),
      orderItems: this.cartItems.map(item => ({
        productId: item.productId,
        cartItemId: item.id,
        productName: item.productName,
        basePrice: item.basePrice,
        discountPercent: item.discountPercent,
        productColorId: item.productColorId,
        colorName: item.colorName,
        colorCode: item.colorCode,
        productVariationId: item.productVariationId,
        size: item.size,
        inventoryId: item.inventoryId,
        quantity: item.quantity,
        imageUrl: item.imageUrl
      })),
      paymentDetail: {
        paymentMethod: this.checkoutForm.get('paymentMethod')?.value,
        amount: this.getFinalTotal(),
        transactionId: undefined
      }
    };
  }

  private handleOrderSuccess(response: any) {
    this.notificationService.success('🎉 Đặt hàng thành công! Cảm ơn bạn đã mua sắm.');
    
    // Delay navigation to show success message
    setTimeout(() => {
      this.router.navigate(['/'], { 
        queryParams: { newOrder: true }
      });
    }, 1500);
  }

  private handleOrderError(error: any) {
    console.error('Order creation error:', error);
    this.isSubmitting = false;
    
    let errorMessage = 'Có lỗi xảy ra khi đặt hàng. Vui lòng thử lại!';
    
    if (error.status === 0) {
      errorMessage = 'Lỗi kết nối mạng. Vui lòng kiểm tra internet và thử lại.';
    } else if (error.status === 400) {
      errorMessage = 'Thông tin đặt hàng không hợp lệ. Vui lòng kiểm tra lại.';
    } else if (error.status === 409) {
      errorMessage = 'Một số sản phẩm đã hết hàng. Vui lòng cập nhật giỏ hàng.';
    } else if (error.status >= 500) {
      errorMessage = 'Lỗi server. Vui lòng thử lại sau ít phút.';
    } else if (error.error?.message) {
      errorMessage = error.error.message;
    }
    
    this.networkError = errorMessage;
    this.notificationService.error(errorMessage);
  }

  // Utility methods for template
  isFieldInvalid(fieldName: string): boolean {
    const field = this.checkoutForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }

  getFieldError(fieldName: string): string {
    return this.formErrors[fieldName] || '';
  }

  isPaymentMethodSelected(methodId: string): boolean {
    return this.checkoutForm.get('paymentMethod')?.value === methodId;
  }
}