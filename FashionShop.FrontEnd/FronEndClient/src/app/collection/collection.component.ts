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
interface PriceRange {
  min: number;
  max: number | null;
  label: string;
}
export interface PaginatedResponse<T> {
  currentPage: number;
  totalPages: number;
  pageSize: number;
  totalCount: number;
  hasPrevious: boolean;
  hasNext: boolean;
  items: T[];
}
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
  // Add Math as a class property
  protected readonly Math = Math;

  @Input() searchTerm: string = '';
  
  selectedSize: string = '';
  sortOption: string = 'price_asc';
  currentPage: number = 1;
  totalPages: number = 1;
  pageSize: number = 16;
  totalCount: number = 0;
  hasPrevious: boolean = false;
  hasNext: boolean = false;

  priceRanges: PriceRange[] = [
    { min: 0, max: 100000, label: '0₫ - 100,000₫' },
    { min: 100000, max: 200000, label: '100,000₫ - 200,000₫' },
    { min: 200000, max: 500000, label: '200,000₫ - 500,000₫' },
    { min: 500000, max: null, label: 'Trên 500,000₫' }
  ];

  sizes = ['XS', 'S', 'M', 'L', 'XL', '2XL', '3XL'];

  products :Product[] = [];  
  allProducts: Product[] = []; // Lưu trữ tất cả sản phẩm gốc
  filteredProducts: Product[] = []; // Lưu trữ sản phẩm đã được lọc
  selectedPriceRange: PriceRange | null = null;
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
  filterByPriceRange(range: PriceRange | null):void{
    
    if(!range) {
      this.selectedPriceRange = null; // reset when user click delete filter

      this.products = this.mapProducts([...this.allProducts]); // Hiển thị tất cả sản phẩm nếu không có range
      return;
    }
    this.selectedPriceRange = range;
    const filteredProducts = this.allProducts.filter(product =>{
      const price = product.basePrice;  
      if (range.max === null) {
        return price >= range.min; // Trả về sản phẩm có giá lớn hơn hoặc bằng min nếu max là null
      }
      return price >= range.min && price <= range.max; // Trả về sản phẩm có giá trong khoảng min và max
    })
    this.products = this.mapProducts(filteredProducts); // Cập nhật sản phẩm đã lọc 

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
    const sortedProducts = [...this.products]; // Reset to all products before sorting  
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
    this.productService.getProductsByCategoryId(categoryId, this.currentPage).subscribe({
      next: (response: PaginatedResponse<Product>) => {
        this.currentPage = response.currentPage;
        this.totalPages = response.totalPages;
        this.pageSize = response.pageSize;
        this.totalCount = response.totalCount;
        this.hasPrevious = response.hasPrevious;
        this.hasNext = response.hasNext;
        
        this.allProducts = response.items;
        this.products = this.mapProducts([...this.allProducts]);

        // Generate page numbers array
        this.generatePageNumbers();

        // Apply existing filters
        if (this.selectedPriceRange) {
          this.filterByPriceRange(this.selectedPriceRange);
        }
        if (this.sortOption !== 'newest') {
          this.sortProducts(this.sortOption);
        }
      },
      error: (error) => {
        console.error('Error fetching products:', error);
      }
    });
  }

  generatePageNumbers() {
    const MAX_VISIBLE_PAGES = 5;
    const pages: number[] = [];
    
    let startPage = Math.max(1, this.currentPage - Math.floor(MAX_VISIBLE_PAGES / 2));
    let endPage = Math.min(this.totalPages, startPage + MAX_VISIBLE_PAGES - 1);

    if (endPage - startPage + 1 < MAX_VISIBLE_PAGES) {
      startPage = Math.max(1, endPage - MAX_VISIBLE_PAGES + 1);
    }

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }

    this.pages = pages;
  }

  goToPage(page: number) {
    if (page !== this.currentPage && page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      const categoryId = this.activatedRoute.snapshot.queryParams['categoryId'];
      if (categoryId) {
        this.getProductsByCategory(categoryId);
      }
    }
  }
}
