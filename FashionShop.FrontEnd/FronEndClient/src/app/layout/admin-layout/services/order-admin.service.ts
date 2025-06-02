import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { AuthAdminService } from './auth-admin.service';

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

export interface OrderPagedList {
  currentPage: number;
  totalPages: number;
  pageSize: number;
  totalCount: number;
  hasPrevious: boolean;
  hasNext: boolean;
  items: AdminOrder[];
}

export interface AdminOrder {
  id: string;
  userId: string;
  address: string;
  status: number;
  total: number;
  fullName: string;
  contactNumber: string;
  createdAt: string;
  orderItems: any[];
  paymentDetail: any;
}
@Injectable({
  providedIn: 'root'
})
export class OrderAdminService {
  private baseUrl = environment.apiUrl;

  constructor(
    private http: HttpClient,
    private authAdminService: AuthAdminService,
    private router: Router
  ) {}

  getAllOrders(pageNumber: number = 1, pageSize: number = 16, orderStatus?: OrderStatus): Observable<OrderPagedList> {
    try {
      const headers = this.authAdminService.getAdminHeaders();
      let params = new HttpParams()
        .set('pageNumber', pageNumber.toString())
        .set('pageSize', pageSize.toString());
      
      if (orderStatus !== undefined) {
        params = params.set('orderStatus', orderStatus.toString());
      }

      return this.http.get<OrderPagedList>(`${this.baseUrl}/order/admin-get-all`, { headers, params })
        .pipe(
          catchError((error: HttpErrorResponse) => {
            if (error.status === 401) {
              this.authAdminService.logout();
              this.router.navigate(['/admin/login']);
            }
            return throwError(() => error);
          })
        );
    } catch (error) {
      this.router.navigate(['/admin/login']);
      return throwError(() => new Error('No authentication token found'));
    }
  }
  updateOrderStatus(orderId: string, newStatus: OrderStatus): Observable<any> {
    const headers = this.authAdminService.getAdminHeaders();
    return this.http.put(`${this.baseUrl}/order/${orderId}/status`, newStatus, { headers })
      .pipe(
        catchError((error: HttpErrorResponse) => {
          if (error.status === 400) {
            return throwError(() => new Error('Không thể cập nhật trạng thái đơn hàng này'));
          }
          return throwError(() => error);
        })
      );
  }

  getValidNextStatuses(currentStatus: OrderStatus): OrderStatus[] {
    const transitions = new Map<OrderStatus, OrderStatus[]>([
      [OrderStatus.Pending, [OrderStatus.Confirmed, OrderStatus.Cancelled]],
      [OrderStatus.Confirmed, [OrderStatus.Shipping, OrderStatus.Cancelled]],
      [OrderStatus.Shipping, [OrderStatus.Delivered]],
      [OrderStatus.Delivered, [OrderStatus.ReturnRequested, OrderStatus.Completed]],
      [OrderStatus.ReturnRequested, [OrderStatus.ReturnApproved, OrderStatus.ReturnRejected]],
      [OrderStatus.ReturnApproved, [OrderStatus.Cancelled]],
      [OrderStatus.ReturnRejected, [OrderStatus.Completed]]
    ]);

    return transitions.get(currentStatus) || [];
  }
}