// cart.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { environment } from "../environments/environment";

interface CartItem {
  id: string;
  cartId: string;
  productId: string;
  productName: string;
  basePrice: number;
  discountPercent: number;
  colorName: string;
  colorCode: string;
  size: string;
  quantity: number;
  imageUrl: string;
  inventoryId: string;
  productColorId: string;
  productVariationId: string;
}


@Injectable({
  providedIn: 'root'
})
export class CartService {
    private apiUrlCartItem = `${environment.apiUrl}/cartitem`;
    private apiUrlCart = `${environment.apiUrl}/cart`;

  constructor(private http: HttpClient) {}
  //cart
  getCartIdByUserId(userId: string): Observable<string> {
    return this.http.get<string>(`
      ${this.apiUrlCart}/GetCartIdByUserId/${userId}`);
  }

  //cart item
  getCartByUserId(cartId: string) {
    return this.http.get<any>(`${this.apiUrlCartItem}/cart/${cartId}`);
  }

  updateCartItem(itemId: string, updateDto: any) {
    return this.http.put(`${this.apiUrlCartItem}/${itemId}`, updateDto);
  }

  removeCartItem(itemId: string) {
    return this.http.delete(`${this.apiUrlCartItem}/${itemId}`);
  }

  addToCart(cartItem: any) {
    return this.http.post(this.apiUrlCartItem, cartItem);
  }
}