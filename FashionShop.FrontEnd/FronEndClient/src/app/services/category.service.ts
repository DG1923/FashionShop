import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "../environments/environment";
import { map } from 'rxjs/operators';
export interface Category {
    id: string;
    name: string;
    imageUrl: string;
    subCategory?: Category[];
}
@Injectable({
    providedIn: 'root'})
export class CategoryService {
    private apiUrl = `${environment.apiUrl}/category`;
    private data : Category[] = [];
    constructor(private http: HttpClient) { }
    getAllCategories():Observable<Category[]> {
        return this.http.get<Category[]>(this.apiUrl);
    }   
    getCategoryById(id: string): Observable<Category> {
        return this.http.get<Category>(`${this.apiUrl}/${id}`);
    }
    getSubCategoryById(id: string ):Observable<Category>{
       return this.http.get<Category>(`${this.apiUrl}/GetSubCategories/${id}`);
    }
    
}