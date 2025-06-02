import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderService } from '../../services/order.service';
import { AuthService } from '../../services/auth.service';
import { Order, OrderStatus } from '../../models/order.model';
import { ToastService } from '../../services/toast.service';
import { FormsModule } from '@angular/forms';

type OrderStatusFilter = OrderStatus | 'returns';

@Component({
  selector: 'app-order',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.css']
})
export class OrderComponent implements OnInit {
  orders: Order[] = [];
  isLoading = false;
  error: string | null = null;
  selectedStatus: OrderStatusFilter = OrderStatus.Pending;
  selectedOrderId: string | null = null;
  returnReason: string = '';
  isSubmittingReturn: boolean = false;
  OrderStatus = OrderStatus; // Make enum available in template

  orderStatuses = [
    { value: OrderStatus.Pending, label: 'Chờ xác nhận' },
    { value: OrderStatus.Confirmed, label: 'Đã xác nhận' },
    { value: OrderStatus.Shipping, label: 'Đang giao' },
    { value: OrderStatus.Delivered, label: 'Đã giao' },
    { value: OrderStatus.Completed, label: 'Hoàn thành' },
    { value: OrderStatus.Cancelled, label: 'Đã hủy' },
    { value: 'returns' as const, label: 'Hoàn trả', isSpecial: true }
  ];

  constructor(
    private orderService: OrderService,
    private authService: AuthService,
    private toastService: ToastService
  ) {}

  ngOnInit() {
    this.loadOrders();
  }

  loadOrders() {
    const userId = this.authService.getUserId();
    if (!userId) {
      this.toastService.error('Vui lòng đăng nhập để xem đơn hàng');
      return;
    }

    this.isLoading = true;
    this.error = null;

    if (this.selectedStatus === 'returns') {
      // Load return-related orders
      this.orderService.getReturnOrders(userId)
        .subscribe({
          next: (orders) => {
            this.orders = orders;
            this.isLoading = false;
          },
          error: (err) => {
            console.error('Error loading return orders:', err);
            this.error = 'Không thể tải đơn hàng hoàn trả. Vui lòng thử lại.';
            this.isLoading = false;
          }
        });
    } else {
      // Existing code for normal order status
      this.orderService.getOrdersByStatus(this.selectedStatus, userId)
        .subscribe({
          next: (orders) => {
            this.orders = orders;
            this.isLoading = false;
          },
          error: (err) => {
            console.error('Error loading orders:', err);
            this.error = 'Không thể tải đơn hàng. Vui lòng thử lại.';
            this.isLoading = false;
          }
        });
    }
  }

  filterByStatus(status: OrderStatusFilter) {
    this.selectedStatus = status;
    this.loadOrders();
  }

  getStatusText(status: OrderStatus): string {
    const statusMap: Record<OrderStatus, string> = {
      [OrderStatus.Pending]: 'Chờ xác nhận',
      [OrderStatus.Confirmed]: 'Đã xác nhận',
      [OrderStatus.Shipping]: 'Đang giao',
      [OrderStatus.Delivered]: 'Đã giao',
      [OrderStatus.ReturnRequested]: 'Yêu cầu trả hàng',
      [OrderStatus.ReturnApproved]: 'Chấp nhận trả hàng',
      [OrderStatus.ReturnRejected]: 'Từ chối trả hàng',
      [OrderStatus.Completed]: 'Hoàn thành',
      [OrderStatus.Cancelled]: 'Đã hủy'
    };
    return statusMap[status] || 'Không xác định';
  }

  getStatusClass(status: OrderStatus): string {
    const statusClassMap: Record<OrderStatus, string> = {
      [OrderStatus.Pending]: 'bg-yellow-100 text-yellow-800',
      [OrderStatus.Confirmed]: 'bg-blue-100 text-blue-800',
      [OrderStatus.Shipping]: 'bg-purple-100 text-purple-800',
      [OrderStatus.Delivered]: 'bg-green-100 text-green-800',
      [OrderStatus.ReturnRequested]: 'bg-orange-100 text-orange-800',
      [OrderStatus.ReturnApproved]: 'bg-teal-100 text-teal-800',
      [OrderStatus.ReturnRejected]: 'bg-red-100 text-red-800',
      [OrderStatus.Completed]: 'bg-green-100 text-green-800',
      [OrderStatus.Cancelled]: 'bg-gray-100 text-gray-800'
    };
    return statusClassMap[status] || 'bg-gray-100 text-gray-800';
  }

  canRequestReturn(status: OrderStatus): boolean {
    return status === OrderStatus.Delivered;
  }

  openReturnModal(orderId: string) {
    this.selectedOrderId = orderId;
    this.returnReason = '';
  }

  closeReturnModal() {
    this.selectedOrderId = null;
    this.returnReason = '';
  }

  submitReturnRequest() {
    if (!this.selectedOrderId || !this.returnReason.trim()) {
      this.toastService.error('Vui lòng nhập lý do trả hàng');
      return;
    }

    this.isSubmittingReturn = true;
    this.orderService.requestReturn(this.selectedOrderId, this.returnReason)
      .subscribe({
        next: (success) => {
          if (success) {
            this.toastService.success('Yêu cầu trả hàng đã được gửi');
            this.loadOrders(); // Reload orders to update status
          } else {
            this.toastService.error('Không thể gửi yêu cầu trả hàng');
          }
          this.closeReturnModal();
        },
        error: (err) => {
          console.error('Error requesting return:', err);
          this.toastService.error('Đã xảy ra lỗi khi gửi yêu cầu');
        },
        complete: () => {
          this.isSubmittingReturn = false;
        }
      });
  }

  getReturnStatusClass(status: OrderStatus): string {
    switch(status) {
      case OrderStatus.ReturnRequested:
        return 'border-l-4 border-orange-500';
      case OrderStatus.ReturnApproved:
        return 'border-l-4 border-green-500';
      case OrderStatus.ReturnRejected:
        return 'border-l-4 border-red-500';
      default:
        return '';
    }
  }
}