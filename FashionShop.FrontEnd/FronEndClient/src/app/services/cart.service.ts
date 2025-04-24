// cart.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';

interface CartItem {
  id: string;
  name: string;
  price: number;
  quantity: number;
  imageUrl: string;
}

@Injectable({
  providedIn: 'root'
})
export class CartService {
  // API endpoints
  private cartApiUrl = '/api/cart';
  
  // Local cart state
  private cartItemsSubject = new BehaviorSubject<CartItem[]>([]);
  cartItems$ = this.cartItemsSubject.asObservable();
  
  // Cache timeout in milliseconds (5 minutes)
  private cacheTimeout = 5 * 60 * 1000;
  private lastFetchTime = 0;
  
  constructor(private http: HttpClient) {
    // Load cart from local storage first for immediate display
    this.loadCartFromLocalStorage();
    // Then fetch from API
    this.fetchCart();
  }
  
  // Get cart items with caching strategy
  getCartItems(): Observable<CartItem[]> {
    const now = Date.now();
    // Refresh from API if cache is expired
    if (now - this.lastFetchTime > this.cacheTimeout) {
      this.fetchCart();
    }
    return this.cartItems$;
  }
  
  // Get cart count for header badge
  getCartCount(): Observable<number> {
    return new Observable<number>(observer => {
      this.cartItems$.subscribe(items => {
        const count = items.reduce((total, item) => total + item.quantity, 0);
        observer.next(count);
      });
    });
  }
  
  // Fetch cart from API
  private fetchCart(): void {
    this.http.get<CartItem[]>(this.cartApiUrl)
      .pipe(
        tap(() => this.lastFetchTime = Date.now()),
        catchError(error => {
          console.error('Error fetching cart', error);
          // Return cached data on error
          return this.cartItems$;
        })
      )
      .subscribe(items => {
        this.cartItemsSubject.next(items);
        this.saveCartToLocalStorage(items);
      });
  }
  
  // Save cart to localStorage for offline access
  private saveCartToLocalStorage(items: CartItem[]): void {
    localStorage.setItem('cart', JSON.stringify(items));
  }
  
  // Load cart from localStorage
  private loadCartFromLocalStorage(): void {
    const savedCart = localStorage.getItem('cart');
    if (savedCart) {
      try {
        const items = JSON.parse(savedCart) as CartItem[];
        this.cartItemsSubject.next(items);
      } catch (e) {
        console.error('Error parsing saved cart', e);
      }
    }
  }
}