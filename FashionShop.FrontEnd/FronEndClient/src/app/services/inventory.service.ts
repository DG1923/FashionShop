import { Injectable } from "@angular/core";
import { environment } from "../environments/environment";
import { Inventory } from "../models/inventory.model";
import { Observable } from "rxjs";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { AuthService } from "./auth.service";

@Injectable({
    providedIn: "root"
})
export class InventoryService {
    private apiUrl = `${environment.apiUrl}/inventory`;

    constructor(
        private http: HttpClient,
        private authService: AuthService
    ) {}

    getInventoryByProductId(productId: string): Observable<Inventory> {
        const token = this.authService.getToken();
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${token}`
        });

        return this.http.get<Inventory>(
            `${this.apiUrl}/getInventoryByProductId/${productId}`,
            { headers }
        );
    }
}