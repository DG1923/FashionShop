import { CommonModule } from '@angular/common';
import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CarouselModule } from 'primeng/carousel';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { ProductCardComponent } from "../product-card/product-card.component";
export interface Product {
  id: string;
  name: string;
  imageUrl: string;
  price: number;
  salePrice?: number;
  rating?: number;       // ví dụ 4.8
  reviewCount?: number;  // ví dụ 154
  labelTag?: string;     // ví dụ "Outlet"
  coolCashTag?: boolean; // bật icon X2 Cool Cash
}

@Component({
  selector: 'app-list-products',
  standalone: true,
  imports: [CommonModule, RouterLink, CarouselModule, ButtonModule, TagModule, ProductCardComponent],
  templateUrl: './list-products.component.html',
  styleUrl: './list-products.component.css'
})
export class ListProductsComponent implements OnInit {
  title:string = 'List Products';
  @ViewChild('scrollContainer') scrollContainer!: ElementRef;
  private scrollInterval: any;
  ngOnInit(): void {
    setTimeout(() => {
      this.isLoading = false;
      this.products = Array.from({ length: 10 }, (_, i) => i + 1);

    }, 3000);
    this.startAutoScroll();
  }  
  products: number[]| undefined;
  isLoading: boolean = true;  
  private startAutoScroll() {
    this.scrollInterval = setInterval(() => {
      const container = this.scrollContainer.nativeElement;
      const scrollAmount = 300; // Adjust this value based on your card width
      
      if (container.scrollLeft + container.clientWidth >= container.scrollWidth) {
        // If reached the end, scroll back to start
        container.scrollTo({ left: 0, behavior: 'smooth' });
      } else {
        // Scroll to next position
        container.scrollTo({
          left: container.scrollLeft + scrollAmount,
          behavior: 'smooth'
        });
      }
    }, 10000); // 3 seconds interval
  }

}
