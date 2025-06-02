import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderAdminService, OrderPagedList, AdminOrder, OrderStatus } from '../../services/order-admin.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-order-management',
  standalone: true,
  imports: [CommonModule,FormsModule],
  templateUrl: './order-managerment.component.html',
  styleUrls: ['./order-managerment.component.css']
})
export class OrderManagementComponent implements OnInit {
  orders: AdminOrder[] = [];
  currentPage = 1;
  pageSize = 16;
  totalPages = 0;
  totalCount = 0;
  isLoading = false;
  error: string | null = null;
  selectedStatus: OrderStatus | undefined = undefined;
  OrderStatus = OrderStatus;
  
  orderStatuses = [
    { value: undefined, label: 'Tất cả' },
    { value: OrderStatus.Pending, label: 'Chờ xác nhận' },
    { value: OrderStatus.Confirmed, label: 'Đã xác nhận' },
    { value: OrderStatus.Shipping, label: 'Đang giao' },
    { value: OrderStatus.Delivered, label: 'Đã giao' },
    { value: OrderStatus.ReturnRequested, label: 'Yêu cầu trả hàng' },
    { value: OrderStatus.ReturnApproved, label: 'Đã chấp nhận trả hàng' },
    { value: OrderStatus.ReturnRejected, label: 'Từ chối trả hàng' },
    { value: OrderStatus.Completed, label: 'Hoàn thành' },
    { value: OrderStatus.Cancelled, label: 'Đã hủy' }
  ];

  selectedOrder: AdminOrder | null = null;
  availableStatuses: OrderStatus[] = [];
  newStatus: OrderStatus | null = null;
  isUpdatingStatus = false;

  constructor(private orderAdminService: OrderAdminService) {}

  ngOnInit() {
    this.loadOrders();
  }

  onStatusChange(status: OrderStatus | undefined) {
    this.selectedStatus = status;
    this.currentPage = 1; // Reset to first page when filtering
    this.loadOrders();
  }

  loadOrders(page: number = this.currentPage) {
    this.isLoading = true;
    this.error = null;

    this.orderAdminService.getAllOrders(page, this.pageSize, this.selectedStatus)
      .subscribe({
        next: (response: OrderPagedList) => {
          this.orders = response.items;
          this.currentPage = response.currentPage;
          this.totalPages = response.totalPages;
          this.totalCount = response.totalCount;
          this.isLoading = false;
        },
        error: (err) => {
          console.error('Error loading orders:', err);
          if (err.status === 401) {
            this.error = 'Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.';
          } else {
            this.error = 'Không thể tải danh sách đơn hàng. Vui lòng thử lại.';
          }
          this.isLoading = false;
        }
      });
  }

  getOrderStatusText(status: OrderStatus): string {
    return this.orderStatuses.find(s => s.value === status)?.label || 'Không xác định';
  }

  getStatusClass(status: OrderStatus): string {
    const statusClassMap: { [key: number]: string } = {
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

  getPages(): number[] {
    const totalNumbers = 5;
    const currentBlock = Math.ceil(this.currentPage / totalNumbers);
    const start = (currentBlock - 1) * totalNumbers + 1;
    const end = Math.min(currentBlock * totalNumbers, this.totalPages);
    
    return Array.from({length: (end - start + 1)}, (_, i) => start + i);
  }

  onPageChange(page: number) {
    if (page >= 1 && page <= this.totalPages && page !== this.currentPage) {
      this.currentPage = page;
      this.loadOrders(page);
    }
  }

  openStatusUpdate(order: AdminOrder) {
    this.selectedOrder = order;
    this.availableStatuses = this.orderAdminService.getValidNextStatuses(order.status);
    this.newStatus = null;
  }

  closeStatusUpdate() {
    this.selectedOrder = null;
    this.availableStatuses = [];
    this.newStatus = null;
  }

  updateOrderStatus() {
    if (!this.selectedOrder || this.newStatus === null) return;

    this.isUpdatingStatus = true;
    this.orderAdminService.updateOrderStatus(this.selectedOrder.id, this.newStatus)
      .subscribe({
        next: () => {
          // Update the order status locally
          if (this.selectedOrder) {
            this.selectedOrder.status = this.newStatus!;
          }
          this.closeStatusUpdate();
          this.loadOrders(this.currentPage); // Reload current page
          this.isUpdatingStatus = false;
        },
        error: (err) => {
          console.error('Error updating order status:', err);
          this.error = err.message || 'Không thể cập nhật trạng thái đơn hàng';
          this.isUpdatingStatus = false;
        }
      });
  }
}