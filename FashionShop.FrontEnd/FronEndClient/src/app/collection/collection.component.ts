import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
@Component({
  selector: 'app-collection',
  standalone: true, 
  templateUrl: './collection.component.html',
  imports: [ 
    FormsModule,CommonModule],
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

  products = [
    {
      name: 'Piqué Biker Jacket',
      price: 67.24,
      oldPrice: null,
      image: 'https://media3.coolmate.me/cdn-cgi/image/width=672,height=990,quality=85/uploads/May2023/combo2tatchaybo-1_87_59.jpg',
      rating: 4,
      onSale: false
    },
    {
      name: 'Multi-pocket Chest Bag',
      price: 43.48,
      oldPrice: 58.99,
      image: 'https://media3.coolmate.me/cdn-cgi/image/width=672,height=990,quality=85/uploads/May2023/combo2tatchaybo-1_87_59.jpg',
      rating: 5,
      onSale: true
    }
    // Add more products as needed
  ];

  pages = [1, 2, 3, 4, 5];

  constructor() { }

  ngOnInit(): void {
  }

  selectSize(size: string): void {
    this.selectedSize = size;
  }

  goToPage(page: number): void {
    this.currentPage = page;
    // Implement pagination logic
  }
}
