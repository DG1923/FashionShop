// cart.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { environment } from "../environments/environment";
import { CartItem, CartItemAddDto } from '../models/cartItem.model';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private apiUrlCartItem = `${environment.apiUrl}/cartitem`;
  private apiUrlCart = `${environment.apiUrl}/cart`;
  private cartCountSubject = new BehaviorSubject<number>(0);
  public cartCount$ = this.cartCountSubject.asObservable();
  cartItems: CartItem[] = [];

  constructor(private http: HttpClient, private authService: AuthService) {}

  getCartCount(): number {
    return this.cartItems.length;
  }

  // Observable để component có thể subscribe
  getCartCount$(): Observable<number> {
    return this.cartCount$;
  }

  updateCartCount(): void {
    const userId = this.authService.getUserId();
    if (userId) {
      // Lấy cartId trước, sau đó lấy cart items
      this.getCartIdByUserId(userId).subscribe({
        next: (cartId) => {
          if (cartId) {
            this.getCartByUserId(cartId).subscribe();
          }
        },
        error: (error) => {
          console.error('Error getting cart ID:', error);
          this.cartCountSubject.next(0);
        }
      });
    } else {
      // Nếu không có userId, set count = 0
      this.cartCountSubject.next(0);
    }
  }

  //cart
  getCartIdByUserId(userId: string): Observable<string> {
    return this.http.get<string>(`${this.apiUrlCart}/GetCartIdByUserId/${userId}`);
  }

  //cart item
  getCartByUserId(cartId: string): Observable<CartItem[]> {
    return this.http.get<CartItem[]>(`${this.apiUrlCartItem}/cart/${cartId}`).pipe(
      tap((items: CartItem[]) => {
        this.cartItems = items;
        // Cập nhật count thông qua BehaviorSubject
        this.cartCountSubject.next(items.length);
      }),
      catchError(error => {
        console.error('Error fetching cart items:', error);
        this.cartCountSubject.next(0);
        return of([]); // Return empty array instead of []
      })
    );
  }

  updateCartItem(itemId: string, updateDto: any): Observable<any> {
    return this.http.put(`${this.apiUrlCartItem}/${itemId}`, updateDto).pipe(
      tap(() => {
        // Refresh cart count after update
        this.updateCartCount();
      })
    );
  }

  removeCartItem(itemId: string): Observable<any> {
    return this.http.delete(`${this.apiUrlCartItem}/${itemId}`).pipe(
      tap(() => {
        // Refresh cart count after removal
        this.updateCartCount();
      })
    );
  }

  addToCart(cartItem: any): Observable<any> {
    return this.http.post(this.apiUrlCartItem, cartItem).pipe(
      tap(() => {
        // Refresh cart count after adding
        this.updateCartCount();
      })
    );
  }

  // Method để clear cart khi logout
  clearCart(): void {
    this.cartItems = [];
    this.cartCountSubject.next(0);
  }
}