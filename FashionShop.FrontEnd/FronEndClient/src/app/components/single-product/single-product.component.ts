import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { ProductDetails,ProductColor,ProductVariation } from '../../models/product.model';
import { CartService } from '../../services/cart.service';
import { ToastService } from '../../services/toast.service';
import { CartItem } from '../../models/cartItem.model';
import { InventoryService } from '../../services/inventory.service';
import { AuthService } from '../../services/auth.service';
import { Inventory } from '../../models/inventory.model';
import { firstValueFrom } from 'rxjs';
@Component({
  selector: 'app-single-product',
  templateUrl: './single-product.component.html',
  standalone: true, 
  imports: [CommonModule, FormsModule],  
})
export class SingleProductComponent implements OnInit {
  product!: ProductDetails;
  inventory!: Inventory;
  cartId!: string; 
  selectedImage: string = '';
  selectedColor: ProductColor | null = null;
  selectedSize: ProductVariation | null = null;
  quantity: number = 1;
  isLoading: boolean = true;
  error: string | null = null;
msgCart: any;
isLoadingToCart: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private cartService: CartService,
    private router: Router,
    private inventoryService: InventoryService,
    private authService: AuthService, // Add this service for user authentication
    private toastService: ToastService // Add this service for notifications
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
    this.msgCart = null; // Reset message when selecting size
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

  async addToCart() {
    if (!this.selectedSize) {
      this.msgCart = 'Vui lòng chọn kích thước';
      return;
    }

    if (this.quantity > this.selectedSize.quantity) {
      this.toastService.error('Số lượng đã vượt quá số lượng có sẵn');
      return;
    }

    const userId = this.authService.getUserId();
    if (!userId) {
      this.toastService.error('Vui lòng đăng nhập để thêm vào giỏ hàng');
      this.router.navigate(['/login']);
      return;
    }

    try {
      // Đặt loading state
      this.isLoadingToCart = true;

      // Đợi lấy inventory
      const inventory = await firstValueFrom(
        this.inventoryService.getInventoryByProductId(this.selectedSize.id)
      );

      if (!inventory || inventory.quantity <= 0) {
        this.toastService.error('Sản phẩm này hiện không có sẵn trong kho');
        return;
      }

      // Đợi lấy cartId
      const cartId = await firstValueFrom(
        this.cartService.getCartIdByUserId(userId)
      );

      if (!cartId) {
        this.toastService.error('Không tìm thấy giỏ hàng của bạn');
        return;
      }

      const cartItem: CartItem = {
        cartId: cartId,
        inventoryId: inventory.inventoryId,
        productId: this.product.id,
        productName: this.product.name,
        basePrice: this.product.basePrice,
        discountPercent: this.product.discountDisplayDTO?.discountPercent || 0,
        productColorId: this.selectedColor!.id,
        colorName: this.selectedColor!.colorName,
        colorCode: this.selectedColor!.colorCode || null,
        productVariationId: this.selectedSize.id,
        size: this.selectedSize.size,
        quantity: this.quantity,
        imageUrl: this.selectedColor!.imageUrlColor
      };
      await new Promise(resolve => setTimeout(resolve, 5000));

      await firstValueFrom(this.cartService.addToCart(cartItem));
      this.toastService.success('Đã thêm vào giỏ hàng');
      this.cartService.updateCartCount();

    } catch (error) {
      console.error('Error adding to cart:', error);
      this.toastService.error('Không thể thêm vào giỏ hàng');
    } finally {
      this.isLoadingToCart = false;
    }
  }
}
