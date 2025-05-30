import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { ProductDetails,ProductColor,ProductVariation } from '../../models/product.model';

@Component({
  selector: 'app-single-product',
  templateUrl: './single-product.component.html',
  standalone: true, 
  imports: [CommonModule, FormsModule],  
})
export class SingleProductComponent implements OnInit {
  product!: ProductDetails;
  selectedImage: string = '';
  selectedColor: ProductColor | null = null;
  selectedSize: ProductVariation | null = null;
  quantity: number = 1;
  isLoading: boolean = true;
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private router: Router
  ) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      const id = params['id'];
      this.loadProductDetails(id);
    });
  }

  loadProductDetails(id: string) {
    this.isLoading = true;
    this.productService.getProductDetails(id).subscribe({
      next: (product) => {
        this.product = product;
        this.selectedImage = product.mainImageUrl;
        if (product.productColorsDisplayDTO?.length > 0) {
          this.selectedColor = product.productColorsDisplayDTO[0];
        }
        this.isLoading = false;
      },
      error: (error) => {
        this.error = 'Không thể tải thông tin sản phẩm';
        this.isLoading = false;
        console.error('Error loading product:', error);
      }
    });
  }

  selectColor(color: ProductColor) {
    this.selectedColor = color;
    this.selectedImage = color.imageUrlColor;
    this.selectedSize = null; // Reset size when color changes
  }

  selectSize(variation: ProductVariation) {
    this.selectedSize = variation;
    // Reset quantity when changing size
    this.quantity = 1;
  }

  incrementQuantity() {
     if(this.quantity <= 0){
      this.quantity = 1; // Reset to 1 if quantity goes below 1
    }
    if (this.selectedSize && this.quantity < this.selectedSize.quantity) {
      this.quantity++;
    }
   
  }

  decrementQuantity() {
    if(this.quantity<0){
      this.quantity = 1; // Reset to 1 if quantity goes below 1
    }
    if (this.quantity > 1) {
      this.quantity--;
    }

  }

  addToCart() {
    if (!this.selectedSize) {
      alert('Vui lòng chọn kích thước');
      return;
    }

    if (this.quantity > this.selectedSize.quantity) {
      alert('Số lượng đã vượt quá số lượng có sẵn');
      return;
    }

    console.log('Added to cart:', {
      product: this.product.name,
      color: this.selectedColor?.colorName,
      size: this.selectedSize.size,
      quantity: this.quantity,
      total: this.product.discountedPrice == null ? 
        this.quantity * this.product.basePrice : 
        this.quantity * this.product.discountedPrice
    });
  }
}
