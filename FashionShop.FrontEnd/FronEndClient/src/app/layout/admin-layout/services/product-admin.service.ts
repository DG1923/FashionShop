import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthAdminService } from './auth-admin.service';

export interface ProductPagedList {
  currentPage: number;
  totalPages: number;
  pageSize: number;
  totalCount: number;
  hasPrevious: boolean;
  hasNext: boolean;
  items: Product[];
}

export interface Product {
  id: string;
  name: string;
  basePrice: number;
  discountedPrice?: number;
  description?: string;
  mainImageUrl: string;
  sku?: string;
  averageRating?: number;
  totalRating?: number;
}

export interface CreateProductDto {
  name: string;
  description: string;
  price: number;
  sku: string;
  mainImageUrl: string;
}

export interface UpdateProductDto {
  name: string;
  price: number;
  description: string;
  mainImageUrl: string;
}

@Injectable({
  providedIn: 'root'
})
export class ProductAdminService {
  private baseUrl = environment.apiUrl;

  constructor(
    private http: HttpClient,
    private authAdminService: AuthAdminService
  ) {}

  getAllProducts(pageNumber: number = 1, pageSize: number = 16): Observable<ProductPagedList> {
    const headers = this.authAdminService.getAdminHeaders();
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<ProductPagedList>(`${this.baseUrl}/products`, { headers, params })
      .pipe(
        catchError(this.handleError)
      );
  }

  searchProducts(searchTerm: string, pageNumber: number = 1, pageSize: number = 16): Observable<ProductPagedList> {
    const headers = this.authAdminService.getAdminHeaders();
    let params = new HttpParams()
      .set('searchTerm', searchTerm)
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<ProductPagedList>(`${this.baseUrl}/products/search`, { headers, params })
      .pipe(catchError(this.handleError));
  }

  createProduct(product: CreateProductDto): Observable<Product> {
    const headers = this.authAdminService.getAdminHeaders();
    return this.http.post<Product>(`${this.baseUrl}/products`, product, { headers })
      .pipe(catchError(this.handleError));
  }

  updateProduct(id: string, product: UpdateProductDto): Observable<Product> {
    try {
      const headers = this.authAdminService.getAdminHeaders();
      return this.http.put<Product>(`${this.baseUrl}/api/products/${id}`, product, { headers })
        .pipe(
          catchError((error: HttpErrorResponse) => {
            if (error.status === 401) {
              console.error('Unauthorized access');
              return throwError(() => new Error('Unauthorized access. Please login again.'));
            }
            return throwError(() => error);
          })
        );
    } catch (error) {
      return throwError(() => error);
    }
  }

  deleteProduct(id: string): Observable<any> {
    try {
      const headers = this.authAdminService.getAdminHeaders();
      return this.http.delete(`${this.baseUrl}/api/products/${id}`, { headers })
        .pipe(
          catchError((error: HttpErrorResponse) => {
            if (error.status === 401) {
              // Handle unauthorized error
              console.error('Unauthorized access');
              return throwError(() => new Error('Unauthorized access. Please login again.'));
            }
            return throwError(() => error);
          })
        );
    } catch (error) {
      return throwError(() => error);
    }
  }

  private handleError(error: HttpErrorResponse) {
    if (error.status === 401) {
      // Handle unauthorized error
      console.error('Unauthorized access');
      return throwError(() => new Error('Unauthorized access. Please login again.'));
    }
    console.error('An error occurred:', error);
    return throwError(() => error);
  }
}