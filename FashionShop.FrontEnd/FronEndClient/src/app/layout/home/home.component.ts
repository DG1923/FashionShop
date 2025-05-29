import { Component, OnInit } from '@angular/core';
import { HeaderComponent } from "../../components/header/header.component";
import { SliderComponent } from "../../slider/slider.component";
import { CategoryComponent } from "../../components/category/category.component";
import { ListProductsComponent } from "../../components/list-products/list-products.component";
import { CollectionComponent } from "../../collection/collection.component";
import { SingleProductComponent } from "../../components/single-product/single-product.component";
import { FooterComponent } from "../../components/footer/footer.component";
import { Product } from "../../models/product.model"; 
import { ProductService } from '../../services/product.service';
import { PaginatedResponse } from '../../models/PaginatedResponse.model';
@Component({
  selector: 'app-home',
  imports: [HeaderComponent, SliderComponent, CategoryComponent, ListProductsComponent, CollectionComponent, SingleProductComponent, FooterComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  standalone: true, 
})
export class HomeComponent implements OnInit {  
  list_product_by_nam:Product[] = []; ;
  list_product_by_nu:Product[] = [];  
  CATEGORY_ID_NAM = '0a2ab5cb-9b63-4193-ba03-396df74d3b2e';
  CATEGORY_ID_NU = '5b61df93-4c11-4e6f-896f-4d7e63989cc3';

  /**
   *
   */
  constructor(private productService: ProductService) {
    
    
  }
  ngOnInit(): void {
    this.loadProductsByCategory(this.CATEGORY_ID_NU);
    this.loadProductsByCategory(this.CATEGORY_ID_NAM);
  }
  loadProductsByCategory(id: string) {
    this.productService.getProductsByCategoryId(id).subscribe({
      next: (products) => {
        console.log('Products loaded:', products);
        if (products.items && products.items.length > 0) {

          if(id== this.CATEGORY_ID_NAM) {
            this.list_product_by_nam = products.items;
          }else if(id == this.CATEGORY_ID_NU) {
            this.list_product_by_nu = products.items;
          }
        } else {
          console.warn('No products found for category:', id);
        }
      },
      error: (error) => {
        console.error('Error loading products:', error);
      }
    });
  }
}
