import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';

interface ProductColor {
  name: string;
  code: string;
}

interface ProductSize {
  name: string;
  available: boolean;
}

@Component({
  selector: 'app-single-product',
  templateUrl: './single-product.component.html',
  standalone: true, 
  imports: [CommonModule,FormsModule],  
})
export class SingleProductComponent implements OnInit {
  product = {
    id: '1',
    name: 'Combo 5 Đôi Tất Nam cổ trung Basics',
    subTitle: 'Co giãn thoải mái',
    price: 99000,
    description: 'Tất nam cổ trung cao cấp',
    images: [
      'https://media3.coolmate.me/cdn-cgi/image/width=672,height=990,quality=85/uploads/May2023/combo2tatchaybo-1_87_59.jpg',
      'https://media3.coolmate.me/cdn-cgi/image/quality=80/uploads/November2023/_CMM9318.jpg',
      'https://media3.coolmate.me/cdn-cgi/image/quality=80/uploads/November2023/23CMCW.TA002.5.jpg',
      'https://media3.coolmate.me/cdn-cgi/image/quality=80/uploads/November2023/23CMCW.TA002.4.jpg'
    ],
    colors: [
      { name: 'Trắng', code: '#FFFFFF' },
      { name: 'Đen', code: '#000000' },
      { name: 'Xám', code: '#808080' }
    ] as ProductColor[],
    sizes: [
      { name: '35-38', available: true },
      { name: '39-42', available: true },
      { name: '43-46', available: false }
    ] as ProductSize[],
    rating: 4.5,
    reviewCount: 62,
    inStock: true
  };

  selectedImage: string = this.product.images[0];
  selectedColor: ProductColor = this.product.colors[0];
  selectedSize: ProductSize | null = null;
  quantity: number = 1;

  constructor() {}

  ngOnInit() {}

  selectSize(size: ProductSize) {
    if (size.available) {
      this.selectedSize = size;
    }
  }

  incrementQuantity() {
    this.quantity++;
  }

  decrementQuantity() {
    if (this.quantity > 1) {
      this.quantity--;
    }
  }

  addToCart() {
    if (!this.selectedSize) {
      alert('Please select a size');
      return;
    }

    console.log('Added to cart:', {
      product: this.product.name,
      color: this.selectedColor.name,
      size: this.selectedSize.name,
      quantity: this.quantity,
      total: this.quantity * this.product.price
    });
  }
}
