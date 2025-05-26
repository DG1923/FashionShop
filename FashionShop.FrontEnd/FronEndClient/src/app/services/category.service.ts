import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "../environments/environment";

export interface Category { 

    id:string;
    name: string;   
    imageUrl: string;
    descriptions?: string;
}
@Injectable({
    providedIn: 'root'})
export class CategoryService {
    private apiUrl = `${environment.apiUrl}/categories`;
    constructor(private http: HttpClient) { }
    getAllCategories():Observable<Category[]> {
        return this.http.get<Category[]>(this.apiUrl);
    }   
    getCategoryById(id: string): Observable<Category> {
        return this.http.get<Category>(`${this.apiUrl}/${id}`);
    }

    
}