// header.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, map, shareReplay, tap } from 'rxjs/operators';

export interface MenuItem {
  isHovered: boolean;
  title: string;
  link: string;
  columns?: MenuColumn[];
}

export interface MenuColumn {
  title?: string;
  link?: string;
  items: MenuSubItem[];
  image?: {
    src: string;
    alt: string;
    caption: string;
    link: string;
  };
}

export interface MenuSubItem {
  title: string;
  link: string;
}

@Injectable({
  providedIn: 'root'
})
export class HeaderService {
  private apiUrl = '/api/navigation';
  private cacheExpirationMs = 5 * 60 * 1000; // 5 minutes cache
  private lastFetchTime = 0;
  
  private menuItemsSubject = new BehaviorSubject<MenuItem[]>([]);
  public menuItems$ = this.menuItemsSubject.asObservable();
  
  // Store fetched data in memory
  private cachedMenuItems: MenuItem[] | null = null;
  
  constructor(private http: HttpClient) {
    // Load from localStorage first
    this.loadMenuFromStorage();
    
    // Then fetch from API
    this.fetchMenuItems();
  }
  
  private loadMenuFromStorage(): void {
    try {
      const storedMenu = localStorage.getItem('menu_data');
      const storedTimestamp = localStorage.getItem('menu_timestamp');
      
      if (storedMenu && storedTimestamp) {
        const timestamp = parseInt(storedTimestamp, 10);
        const now = Date.now();
        
        // Check if cache is still valid
        if (now - timestamp < this.cacheExpirationMs) {
          const parsedMenu = JSON.parse(storedMenu) as MenuItem[];
          this.menuItemsSubject.next(parsedMenu);
          this.cachedMenuItems = parsedMenu;
          this.lastFetchTime = timestamp;
        }
      }
    } catch (error) {
      console.error('Error loading menu from storage', error);
    }
  }
  
  private saveMenuToStorage(menuItems: MenuItem[]): void {
    try {
      localStorage.setItem('menu_data', JSON.stringify(menuItems));
      localStorage.setItem('menu_timestamp', Date.now().toString());
    } catch (error) {
      console.error('Error saving menu to storage', error);
    }
  }
  
  public fetchMenuItems(): Observable<MenuItem[]> {
    const now = Date.now();
    
    // Return cached data if it's fresh enough
    if (this.cachedMenuItems && now - this.lastFetchTime < this.cacheExpirationMs) {
      return of(this.cachedMenuItems);
    }
    
    // Otherwise fetch from API
    return this.http.get<MenuItem[]>(this.apiUrl).pipe(
      tap(menuItems => {
        this.menuItemsSubject.next(menuItems);
        this.cachedMenuItems = menuItems;
        this.lastFetchTime = now;
        this.saveMenuToStorage(menuItems);
      }),
      catchError(error => {
        console.error('Error fetching menu items', error);
        
        // Return cached data on error, even if expired
        if (this.cachedMenuItems) {
          return of(this.cachedMenuItems);
        }
        
        // Return empty array if no cached data
        return of([]);
      }),
      // Use shareReplay to avoid multiple HTTP requests for the same data
      shareReplay(1)
    );
  }
  
  // Force refresh cache (e.g., after user login/logout)
  public refreshMenuItems(): Observable<MenuItem[]> {
    this.cachedMenuItems = null;
    this.lastFetchTime = 0;
    localStorage.removeItem('menu_data');
    localStorage.removeItem('menu_timestamp');
    
    return this.fetchMenuItems();
  }
}