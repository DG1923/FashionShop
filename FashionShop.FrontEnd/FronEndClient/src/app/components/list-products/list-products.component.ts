import { CommonModule } from '@angular/common';
import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CarouselModule } from 'primeng/carousel';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { ProductCardComponent } from "../product-card/product-card.component";
import { Product } from '../../models/product.model';


@Component({
  selector: 'app-list-products',
  standalone: true,
  imports: [CommonModule, RouterLink, CarouselModule, ButtonModule, TagModule, ProductCardComponent],
  templateUrl: './list-products.component.html',
  styleUrl: './list-products.component.css'
})
export class ListProductsComponent  implements OnInit {
   @Input() products: Product[] = [];
  @Input() title: string = 'Sản phẩm';
  @Input() showViewAll: boolean = false;
  @Input() viewAllLink: string = '/collection';
  isLoading: boolean = false; 
   ngOnInit(): void {
    if(!this.products || this.products.length === 0) {
      this.isLoading = true;
      // Simulate loading data
      setTimeout(() => {
        this.isLoading = false;
      }, 1000);
    }
  }

}
