import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { BehaviorSubject, Observable, of } from "rxjs";
import { environment } from "../environments/environment";
import { map, tap } from 'rxjs/operators';
import { Category } from "../models/category.model";
@Injectable({
    providedIn: 'root'})
export class CategoryService {
     private apiUrl = `${environment.apiUrl}/category`;
    private categoryCache = new Map<string, Category>();
    private mainCategories$ = new BehaviorSubject<Category[]>([]);

    constructor(private http: HttpClient) {
        // Load main categories when service is initialized
        this.loadMainCategories();
    }

    private loadMainCategories() {
        this.http.get<Category[]>(this.apiUrl).pipe(
            tap(categories => {
                this.mainCategories$.next(categories);
                // Cache each category
                categories.forEach(cat => {
                    this.categoryCache.set(cat.id, cat);
                });
            })
        ).subscribe();
    }

    getMainCategories(): Observable<Category[]> {
        return this.mainCategories$.asObservable();
    }

    getSubCategoryById(id: string): Observable<Category> {
        // Check cache first
        const cached = this.categoryCache.get(id);
        if (cached && cached.subCategory) {
            console.log(`Returning cached category for ID: ${id}`);
            return of(cached);
        }

        console.log(`Fetching category from API for ID: ${id}`);
        // If not in cache, fetch from API
        return this.http.get<Category>(`${this.apiUrl}/GetSubCategories/${id}`).pipe(
            tap(category => {
                this.categoryCache.set(id, category);
            }),
            
        );
    }
}