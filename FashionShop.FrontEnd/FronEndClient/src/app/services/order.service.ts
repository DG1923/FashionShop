import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from "../environments/environment";
import { Observable } from "rxjs";
import { HttpHeaders } from "@angular/common/http";
import { AuthService } from "./auth.service";

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private baseUrl = environment.apiUrl;
  // private baseUrl = "https://localhost:7167";

  constructor(private http: HttpClient,
     private authService: AuthService
  ) {}

  createOrder(cartId: string, orderData: any) {
    const token = this.authService.getToken();
            const headers = new HttpHeaders({
                'Authorization': `Bearer ${token}`
            });
    return this.http.post(`${this.baseUrl}/order?cartId=${cartId}`, orderData,{headers});
  }
}