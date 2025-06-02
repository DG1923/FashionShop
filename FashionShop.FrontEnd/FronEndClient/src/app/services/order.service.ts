import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';
import { Observable, forkJoin } from 'rxjs';
import { HttpHeaders } from '@angular/common/http';
import { AuthService } from './auth.service';
import { Order, OrderStatus } from '../models/order.model';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  private baseUrl = environment.apiUrl;
  // private baseUrl = "https://localhost:7167";

  constructor(private http: HttpClient, private authService: AuthService) {}

  createOrder(cartId: string, orderData: any) {
    const token = this.authService.getToken();
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.post(`${this.baseUrl}/order?cartId=${cartId}`, orderData, {
      headers,
    });
  }
  getOrdersByStatus(status: OrderStatus, userId: string): Observable<Order[]> {
    const headers = this.getAuthHeaders();
    return this.http.get<Order[]>(`${this.baseUrl}/order/status/${status}?userId=${userId}`, { headers });
  }

  private getAuthHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }
  requestReturn(orderId: string, reason: string): Observable<boolean> {
    const headers = this.getAuthHeaders();
    return this.http.post<boolean>(`${this.baseUrl}/order/${orderId}/return-request`, 
      { reason },
      { headers }
    );
  }
  getReturnOrders(userId: string): Observable<Order[]> {
    const headers = this.getAuthHeaders();
    const returnStatuses = [
      OrderStatus.ReturnRequested,
      OrderStatus.ReturnApproved,
      OrderStatus.ReturnRejected
    ];
    
    // Create an array of observables for each status
    const requests = returnStatuses.map(status =>
      this.getOrdersByStatus(status, userId)
    );
    
    // Combine all results
    return forkJoin(requests).pipe(
      map(results => results.flat())
    );
  }
}
