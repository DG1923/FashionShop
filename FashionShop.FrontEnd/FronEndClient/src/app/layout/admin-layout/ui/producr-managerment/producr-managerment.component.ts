import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductAdminService, Product, ProductPagedList, CreateProductDto, UpdateProductDto } from '../../services/product-admin.service';
import { ClickOutsideDirective } from '../../services/clickOutside';
@Component({
  selector: 'app-producr-managerment',
  standalone: true,
  imports: [CommonModule, FormsModule,ClickOutsideDirective],
  templateUrl: './producr-managerment.component.html',
  styleUrls: ['./producr-managerment.component.css']
})
export class ProducrManagermentComponent implements OnInit {
  products: Product[] = [];
  currentPage = 1;
  pageSize = 16;
  totalPages = 0;
  totalCount = 0;
  isLoading = false;
  error: string | null = null;
  searchTerm: string = '';
  showCreateModal = false;
  showEditModal = false;
  showDetailModal = false;
  selectedProduct: Product | null = null;
  editForm: UpdateProductDto = {
    name: '',
    price: 0,
    description: '',
    mainImageUrl: ''
  };

  createForm: CreateProductDto = {
    name: '',
    description: '',
    price: 0,
    sku: '',
    mainImageUrl: ''
  };

  constructor(private productService: ProductAdminService) {}

  ngOnInit() {
    this.loadProducts();
  }

  search() {
    if (this.searchTerm.trim()) {
      this.productService.searchProducts(this.searchTerm, this.currentPage, this.pageSize)
        .subscribe(this.handleProductResponse.bind(this));
    } else {
      this.loadProducts();
    }
  }

  loadProducts(page: number = this.currentPage) {
    this.isLoading = true;
    this.error = null;

    this.productService.getAllProducts(page, this.pageSize)
      .subscribe(this.handleProductResponse.bind(this));
  }

  private handleProductResponse(response: ProductPagedList) {
    this.products = response.items;
    this.currentPage = response.currentPage;
    this.totalPages = response.totalPages;
    this.totalCount = response.totalCount;
    this.isLoading = false;
  }

  openCreateModal() {
    this.showCreateModal = true;
  }

  toggleMenu(product: Product) {
    if (this.selectedProduct?.id === product.id) {
      this.selectedProduct = null;
    } else {
      this.selectedProduct = product;
    }
  }

  openEditModal(product: Product) {
    this.selectedProduct = product;
    this.editForm = {
      name: product.name,
      price: product.basePrice,
      description: product.description || '',
      mainImageUrl: product.mainImageUrl
    };
    this.showEditModal = true;
  }

  openDetailModal(product: Product) {
    this.selectedProduct = product;
    this.showDetailModal = true;
  }

  closeDetailModal() {
    this.showDetailModal = false;
    this.selectedProduct = null;
  }

  createProduct() {
    if (!this.validateForm(this.createForm)) return;

    this.isLoading = true;
    this.productService.createProduct(this.createForm)
      .subscribe({
        next: () => {
          this.showCreateModal = false;
          this.loadProducts();
          this.resetCreateForm();
        },
        error: (error) => {
          this.error = 'Không thể tạo sản phẩm';
          this.isLoading = false;
        }
      });
  }

  updateProduct() {
    if (!this.selectedProduct || !this.validateForm(this.editForm)) return;

    this.isLoading = true;
    this.productService.updateProduct(this.selectedProduct.id, this.editForm)
      .subscribe({
        next: () => {
          this.showEditModal = false;
          this.loadProducts();
          this.selectedProduct = null;
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error updating product:', error);
          this.error = 'Không thể cập nhật sản phẩm';
          this.isLoading = false;
        }
      });
  }

  confirmDelete(product: Product) {
    if (confirm(`Bạn có chắc chắn muốn xóa sản phẩm "${product.name}"?`)) {
      this.deleteProduct(product.id);
    }
  }

  deleteProduct(id: string) {
    this.isLoading = true;
    this.productService.deleteProduct(id)
      .subscribe({
        next: () => {
          this.loadProducts();
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error deleting product:', error);
          this.error = 'Không thể xóa sản phẩm';
          this.isLoading = false;
        }
      });
  }

  private validateForm(form: UpdateProductDto): boolean {
    return !!(form.name && form.price > 0 && form.mainImageUrl);
  }

  private resetCreateForm() {
    this.createForm = {
      name: '',
      description: '',
      price: 0,
      sku: '',
      mainImageUrl: ''
    };
  }

  getPages(): number[] {
    const totalNumbers = 5;
    const currentBlock = Math.ceil(this.currentPage / totalNumbers);
    const start = (currentBlock - 1) * totalNumbers + 1;
    const end = Math.min(currentBlock * totalNumbers, this.totalPages);
    
    return Array.from({length: (end - start + 1)}, (_, i) => start + i);
  }

  onPageChange(page: number) {
    if (page >= 1 && page <= this.totalPages && page !== this.currentPage) {
      this.currentPage = page;
      this.loadProducts(page);
    }
  }

  formatPrice(price: number): string {
    return new Intl.NumberFormat('vi-VN', { 
      style: 'currency', 
      currency: 'VND' 
    }).format(price);
  }

  // @HostListener('document:click')
  // closeMenu() {
  //   this.selectedProduct = null;
  // }
}