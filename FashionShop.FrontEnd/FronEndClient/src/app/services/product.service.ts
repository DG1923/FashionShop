import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { map } from 'rxjs/operators';
import { Product, ProductDetails } from '../models/product.model';
import { PaginatedResponse } from '../models/PaginatedResponse.model'; // Assuming you have this model defined
@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private apiUrl = `${environment.apiUrl}/products`;
  private data: Product[] = [];
  constructor(private http: HttpClient) {}
  getAllProducts(
    pageNumber: number = 1
  ): Observable<PaginatedResponse<Product>> {
    return this.http.get<PaginatedResponse<Product>>(
      `${this.apiUrl}?pageNumber=${pageNumber}`
    );
  }
  getProductById(id: string): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/${id}`);
  }
  getProductsByCategoryId(
    categoryId: string,
    pageNumber: number = 1
  ): Observable<PaginatedResponse<Product>> {
    return this.http.get<PaginatedResponse<Product>>(
      `${this.apiUrl}/by-category/${categoryId}?pageNumber=${pageNumber}`
    );
  }
  getProductDetails(id: string): Observable<ProductDetails> {
    return this.http.get<ProductDetails>(`${this.apiUrl}/${id}/details`);
  }
}
