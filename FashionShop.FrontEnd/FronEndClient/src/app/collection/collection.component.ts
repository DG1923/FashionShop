import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../services/product.service';
import { ProductCardComponent } from "../components/product-card/product-card.component";
import { Category } from '../models/category.model';
import { Product } from '../models/product.model';
import { fromEvent } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, finalize } from 'rxjs/operators';

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

  // Loading states
  isLoadingProducts: boolean = false;
  isSearching: boolean = false;
  isSorting: boolean = false;
  isPaginating: boolean = false;
  isLoadingMain: boolean = false;
  priceRanges: PriceRange[] = [
    { min: 0, max: 100000, label: '0₫ - 100,000₫' },
    { min: 100000, max: 200000, label: '100,000₫ - 200,000₫' },
    { min: 200000, max: 500000, label: '200,000₫ - 500,000₫' },
    { min: 500000, max: null, label: 'Trên 500,000₫' }
  ];

  sizes = ['XS', 'S', 'M', 'L', 'XL', '2XL', '3XL'];

  products: Product[] = [];  
  allProducts: Product[] = []; // Lưu trữ tất cả sản phẩm gốc
  filteredProducts: Product[] = []; // Lưu trữ sản phẩm đã được lọc
  selectedPriceRange: PriceRange | null = null;
  pages = [1, 2, 3, 4, 5];

  constructor(private activatedRoute: ActivatedRoute, private productService: ProductService) { }

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe(params => {
      const categoryId = params['categoryId'];
      const searchTerm = params['search'];
      
      if (searchTerm) {
        this.searchProducts(searchTerm);
      } else if (categoryId) {
        this.getProductsByCategory(categoryId);
      } else {
        this.getAllProducts();
      }
    });

    // Listen for search changes
    this.onSearch();

    // Listen for sort changes
    this.watchSortChanges();
  }
  getAllProducts() {
    this.isLoadingProducts = true;  
    this.productService.getAllProducts(this.currentPage)
    .pipe(
      finalize(() => {
        this.isLoadingProducts = false;
      })
    ).subscribe({
      next: (response: PaginatedResponse<Product>) => {
        // Add a small delay to show loading animation
        setTimeout(() => {
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
        }, 600); // Minimum loading time for better UX
      },
      error: (error) => {
        console.error('Error fetching products:', error);
        // You might want to show an error message to the user here
      } 
    })
  }

  // TrackBy functions for better performance
  trackByProduct(index: number, product: Product): any {
    return product.id;
  }

  trackByPriceRange(index: number, range: PriceRange): any {
    return range.label;
  }

  trackByPage(index: number, page: number): any {
    return page;
  }

  filterByPriceRange(range: PriceRange | null): void {
    if (this.isLoadingProducts) return;

    // Add a small delay to show smooth transition
    this.isSorting = true;
    
    setTimeout(() => {
      if (!range) {
        this.selectedPriceRange = null;
        this.products = this.mapProducts([...this.allProducts]);
        this.isSorting = false;
        return;
      }

      this.selectedPriceRange = range;
      const filteredProducts = this.allProducts.filter(product => {
        const price = product.basePrice;  
        if (range.max === null) {
          return price >= range.min;
        }
        return price >= range.min && price <= range.max;
      });
      
      this.products = this.mapProducts(filteredProducts);
      this.isSorting = false;
    }, 300);
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
    if (this.isLoadingProducts) return;

    this.isSorting = true;
    
    // Add delay for smooth animation
    setTimeout(() => {
      const sortedProducts = [...this.products];
      
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
      
      this.products = this.mapProducts(sortedProducts);
      this.isSorting = false;
    }, 400);
  }

  onSearch(): void {
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
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .replace(/đ/g, 'd')
      .replace(/\s+/g, ' ')
      .trim();
  }

  filterProducts(searchTerm: string): void {
    if (this.isLoadingProducts) return;

    this.isSearching = true;
    
    setTimeout(() => {
      if (!searchTerm) {
        this.products = this.mapProducts([...this.allProducts]);
        this.isSearching = false;
        return;
      }

      const normalizedSearchTerm = this.normalizeText(searchTerm);
      
      const filteredProducts = this.allProducts.filter(product => {
        const normalizedProductName = this.normalizeText(product.name);
        return normalizedProductName.includes(normalizedSearchTerm);
      });

      this.products = this.mapProducts(filteredProducts);
      this.isSearching = false;
    }, 500);
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
    this.isLoadingProducts = true;
    
    this.productService.getProductsByCategoryId(categoryId, this.currentPage)
      .pipe(
        finalize(() => {
          this.isLoadingProducts = false;
        })
      )
      .subscribe({
        next: (response: PaginatedResponse<Product>) => {
          // Add a small delay to show loading animation
          setTimeout(() => {
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
          }, 600); // Minimum loading time for better UX
        },
        error: (error) => {
          console.error('Error fetching products:', error);
          // You might want to show an error message to the user here
        }
      });
  }

  searchProducts(term: string) {
    this.isLoadingProducts = true;
    
    this.productService.searchProducts(term, this.currentPage)
      .pipe(
        finalize(() => {
          this.isLoadingProducts = false;
        })
      )
      .subscribe({
        next: (response: PaginatedResponse<Product>) => {
          setTimeout(() => {
            this.updateProductsState(response);
          }, 600);
        },
        error: (error) => {
          console.error('Error searching products:', error);
        }
      });
  }

  private updateProductsState(response: PaginatedResponse<Product>) {
    this.currentPage = response.currentPage;
    this.totalPages = response.totalPages;
    this.pageSize = response.pageSize;
    this.totalCount = response.totalCount;
    this.hasPrevious = response.hasPrevious;
    this.hasNext = response.hasNext;
    
    this.allProducts = response.items;
    this.products = this.mapProducts([...this.allProducts]);
    
    this.generatePageNumbers();
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
  if (page !== this.currentPage && page >= 1 && page <= this.totalPages && !this.isPaginating) {
    this.isPaginating = true;
    this.currentPage = page;
    
    const categoryId = this.activatedRoute.snapshot.queryParams['categoryId'];
    if (categoryId) {
      this.getProductsByCategory(categoryId);
    } else {
      this.getAllProducts();
    }
    
    setTimeout(() => {
      this.isPaginating = false;
    }, 800);
  }
}
}