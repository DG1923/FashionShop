import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthAdminService } from './auth-admin.service';

export interface TopSellingProduct {
  productId: string;
  productName: string;
  totalQuantitySold: number;
  totalRevenue: number;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private baseUrl = environment.apiUrl;

  constructor(
    private http: HttpClient,
    private authAdminService: AuthAdminService
  ) {}

  getTotalRevenue(): Observable<number> {
    const headers = this.authAdminService.getAdminHeaders();
    return this.http.get<number>(`${this.baseUrl}/order/revenue/total`, { headers });
  }

  getTopSellingProducts(): Observable<TopSellingProduct[]> {
    const headers = this.authAdminService.getAdminHeaders();
    return this.http.get<TopSellingProduct[]>(`${this.baseUrl}/order/top-selling`, {
      headers,
      params: { limit: '10' }
    });
  }
}