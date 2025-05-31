// cart.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { environment } from "../environments/environment";
import { CartItem } from '../models/cartItem.model';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private apiUrlCartItem = `${environment.apiUrl}/cartitem`;
  private apiUrlCart = `${environment.apiUrl}/cart`;
  private cartCountSubject = new BehaviorSubject<number>(0);
  cartCount$ = this.cartCountSubject.asObservable();

  constructor(private http: HttpClient) {}

  updateCartCount(count?: number) {
    if (count !== undefined) {
      this.cartCountSubject.next(count);
      return;
    }

    // If count not provided, fetch from API
    const userId = localStorage.getItem('userId');
    if (userId) {
      this.getCartIdByUserId(userId).subscribe(cartId => {
        this.getCartByUserId(cartId).subscribe(cart => {
          this.cartCountSubject.next(cart.items.length);
        });
      });
    }
  }

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