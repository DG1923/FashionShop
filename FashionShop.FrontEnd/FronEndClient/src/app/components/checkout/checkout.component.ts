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

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.css']
})
export class CheckoutComponent implements OnInit {
  checkoutForm!: FormGroup;
  isSubmitting = false;
  cartItems: any[] = [];
  cartId: string = '';
  total: number = 0;

  paymentMethods = [
    { id: 'COD', name: 'Thanh toán khi nhận hàng', icon: 'cash' },
    { id: 'MOMO', name: 'Ví Momo', icon: 'momo' },
    { id: 'VNPAY', name: 'VNPay', icon: 'vnpay' }
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

    this.route.queryParams.subscribe(params => {
      this.cartId = params['cartId'];
      if (!this.cartId) {
        this.router.navigate(['/cart']);
        return;
      }
      this.loadCart();
    });
    this.checkoutForm = this.fb.group({
      fullName: ['', [Validators.required]],
      contactNumber: ['', [Validators.required, Validators.pattern('^[0-9]{10}$')]],
      address: ['', [Validators.required]],
      paymentMethod: ['COD', [Validators.required]]
    });

    this.loadCart();
  }

  loadCart() {
    const userId = this.authService.getUserId();
    if (!userId) {
      this.router.navigate(['/login']);
      return;
    }

    this.cartService.getCartIdByUserId(userId).subscribe({
      next: (cartId) => {
        this.cartId = cartId;
        this.cartService.getCartByUserId(cartId).subscribe({
          next: (items) => {
            this.cartItems = items;
            this.calculateTotal();
          },
          error: (error) => {
            console.error('Error loading cart items:', error);
            this.notificationService.error('Không thể tải giỏ hàng');
          }
        });
      },
      error: (error) => {
        console.error('Error getting cart ID:', error);
        this.notificationService.error('Không thể tìm thấy giỏ hàng');
      }
    });
  }

  calculateTotal(): number {
    return this.cartItems.reduce((total, item) => {
      const price = item.discountPercent ? 
        item.basePrice * (1 - item.discountPercent / 100) : 
        item.basePrice;
      return total + (price * item.quantity);
    }, 0);
  }

  onSubmit() {
    if (this.checkoutForm.valid && this.cartItems.length > 0) {
      this.isSubmitting = true;

      const userId = this.authService.getUserId();
      if (!userId) {
        this.router.navigate(['/login']);
        return;
      }

      const orderData: OrderCreateDto = {
        userId: userId,
        fullName: this.checkoutForm.get('fullName')?.value,
        contactNumber: this.checkoutForm.get('contactNumber')?.value,
        address: this.checkoutForm.get('address')?.value,
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
          amount: this.calculateTotal(),
          transactionId: undefined
        }
      };
      console.log('Order Data:', orderData);

      this.orderService.createOrder(this.cartId, orderData).subscribe({
        next: (response) => {
          this.notificationService.success('Đặt hàng thành công!');
          this.router.navigate(['/']);
        },
        error: (error) => {
          this.isSubmitting = false;
          console.error('Error creating order:', error);
          this.notificationService.error('Có lỗi xảy ra. Vui lòng thử lại!');
        }
      });
    } else {
      this.notificationService.warning('Vui lòng điền đầy đủ thông tin!');
    }
  }
}
