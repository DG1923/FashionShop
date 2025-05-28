import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../services/product.service';
import { ProductCardComponent } from "../components/product-card/product-card.component";
import { Category } from '../models/category.model';
import { Product } from '../models/product.model';
import { fromEvent } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';

@Component({
  selector: 'app-collection',
  standalone: true, 
  templateUrl: './collection.component.html',
  imports: [
    FormsModule, CommonModule,
    ProductCardComponent
],
})
export class CollectionComponent implements OnInit {
  @Input() searchTerm: string = '';
  selectedSize: string = '';
  sortOption: string = 'price_asc';
  currentPage: number = 1;

  categories = [
    { name: 'Men', count: 20 },
    { name: 'Women', count: 30 },
    { name: 'Bags', count: 15 },
    { name: 'Clothing', count: 45 },
    { name: 'Shoes', count: 25 },
    { name: 'Accessories', count: 18 }
  ];

  brands = [
    'Louis Vuitton',
    'Chanel',
    'Hermes',
    'Gucci'
  ];

  priceRanges = [
    '$0.00 - $50.00',
    '$50.00 - $100.00',
    '$100.00 - $150.00',
    '$150.00 - $200.00',
    '$200.00+'
  ];

  sizes = ['XS', 'S', 'M', 'L', 'XL', '2XL', '3XL'];

  products :Product[] = [];  
  allProducts: Product[] = []; // Lưu trữ tất cả sản phẩm gốc
  filteredProducts: Product[] = []; // Lưu trữ sản phẩm đã được lọc

  pages = [1, 2, 3, 4, 5];

  constructor(private activatedRoute:ActivatedRoute,private productService:ProductService) { }

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe(params =>{
      const categoryId = params['categoryId'];
      if (categoryId) {
        this.getProductsByCategory(categoryId);
      }
    });

    // Listen for search changes
    this.onSearch();

    //listen for sort changes
    this.watchSortChanges();
  }
  watchSortChanges() {
    const sortSelect = document.querySelector('#sortSelect');
    if (sortSelect) {
      fromEvent(sortSelect, 'change')
        .pipe(
          map((event: any) => event.target.value)
        )
        .subscribe((sortOption: string) => {
          this.sortProducts(sortOption);
        });
    }
  }
  sortProducts(sortOption: string) {
    const sortedProducts = [...this.allProducts]; // Reset to all products before sorting  
    switch (sortOption) {
      case 'price_asc':
        sortedProducts.sort((a, b) => a.basePrice - b.basePrice);
        break;
      case 'price_desc':
        sortedProducts.sort((a, b) => b.basePrice - a.basePrice);
        break;
      default:
        break;
    }
    this.products = this.mapProducts(sortedProducts); // Map lại sau khi sort 
  }

  onSearch(): void {
    // Debounce search to avoid too many calls
    const searchInput = document.querySelector('#searchInput');
    if (searchInput) {
      fromEvent(searchInput, 'input')
        .pipe(
          debounceTime(300),
          distinctUntilChanged(),
          map((event: any) => event.target.value)
        )
        .subscribe((searchTerm: string) => {
          this.filterProducts(searchTerm);
        });
    }
  }

  // Thêm hàm chuẩn hóa text
  private normalizeText(text: string): string {
    return text
      .toLowerCase()
      .normalize('NFD')                   // Tách dấu thành ký tự riêng
      .replace(/[\u0300-\u036f]/g, '')   // Loại bỏ dấu
      .replace(/đ/g, 'd')                // Thay đổi đ thành d
      .replace(/\s+/g, ' ')              // Chuẩn hóa khoảng trắng
      .trim();
  }

  filterProducts(searchTerm: string): void {
    if (!searchTerm) {
      this.products = this.mapProducts([...this.allProducts]);
      return;
    }

    const normalizedSearchTerm = this.normalizeText(searchTerm);
    
    const filteredProducts = this.allProducts.filter(product => {
      const normalizedProductName = this.normalizeText(product.name);
      return normalizedProductName.includes(normalizedSearchTerm);
    });

    // Áp dụng mapping sau khi filter
    this.products = this.mapProducts(filteredProducts);
  }

  // Tách logic mapping ra thành method riêng để tái sử dụng
  private mapProducts(products: Product[]): Product[] {
    return products.map(product => ({
      id: product.id,
      name: product.name,
      basePrice: product.basePrice,
      mainImageUrl: product.mainImageUrl,
      averageRating: 3.2,
      discountDisplayDTO: product.discountDisplayDTO,
      discountedPrice: product.basePrice - (product.basePrice) * 0.2,
      totalRating: 10,
    }));
  }

  // Cập nhật lại getProductsByCategory
  getProductsByCategory(categoryId: string) {
    this.productService.getProductsByCategoryId(categoryId).subscribe({
      next: (products) => {
        this.allProducts = products;
        this.products = this.mapProducts([...this.allProducts]);
        console.log('Products fetched by category:', this.products);

        // Lọc sản phẩm theo sort option
        if(this.sortOption !=='default'){
          this.sortProducts(this.sortOption);
        }
      },
      error: (error) => {
        console.error('Error fetching products by category:', error);
      }
    });
  }

  selectSize(size: string): void {
    this.selectedSize = size;
  }

  goToPage(page: number): void {
    this.currentPage = page;
    // Implement pagination logic
  }
}
