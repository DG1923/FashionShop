import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "../environments/environment";
import { map } from 'rxjs/operators';
import { Product } from "../models/product.model";  
@Injectable({
    providedIn: 'root'})
export class ProductService {
    private apiUrl = `${environment.apiUrl}/products`;
    private data : Product[] = [];
    constructor(private http: HttpClient) { }
    getAllProducts(): Observable<Product[]> {
        return this.http.get<Product[]>(this.apiUrl);
    }
    getProductById(id: string): Observable<Product> {
        return this.http.get<Product>(`${this.apiUrl}/${id}`);
    }
    getProductsByCategoryId(categoryId: string): Observable<Product[]> {
        return this.http.get<Product[]>(`${this.apiUrl}/by-category/${categoryId}`);
    }
    
}