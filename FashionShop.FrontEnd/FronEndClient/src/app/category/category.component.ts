import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
interface Category {
  id: string;
  name: string;
  imageUrl: string;
  link: string;
  displayOrder: number;
  gender: 'male' | 'female';
}
@Component({
  selector: 'app-category',
  imports: [CommonModule,RouterModule],
  standalone:true,
  templateUrl: './category.component.html',
  styleUrl: './category.component.css'
})
export class CategoryComponent implements OnInit {
  categories: Category[] = [];
  filteredCategories: Category[] = [];
  selectedGender: 'male' | 'female' = 'male';
  isFadeIn = false; 
  isLoading = false;
  constructor() { }

  ngOnInit(): void {
    // Simulating API call with delay for demonstration
    this.isLoading = true;
    this.isFadeIn = true;
    setTimeout(() => {
      this.loadCategories();
      this.isLoading = false;
    }, 300);
  }

  loadCategories(): void {
    // In a real application, this would be retrieved from a service
    this.categories = [
      {
        id: '1',
        name: 'ÁO THUN',
        imageUrl: 'https://images.unsplash.com/photo-1559583985-c80d8ad9b29f?w=900&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxjb2xsZWN0aW9uLXBhZ2V8MXwxMDY1OTc2fHxlbnwwfHx8fHw%3D',
        link: '/collection/ao-thun-nam',
        displayOrder: 1,
        gender: 'male'
      },
      {
        id: '2',
        name: 'ÁO POLO',
        imageUrl: 'https://images.unsplash.com/photo-1502989642968-94fbdc9eace4?q=80&w=3088&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D',
        link: '/collection/ao-polo-nam',
        displayOrder: 2,
        gender: 'male'
      },
      {
        id: '3',
        name: 'QUẦN SHORT',
        imageUrl: 'https://images.unsplash.com/photo-1502989642968-94fbdc9eace4?q=80&w=3088&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D',
        link: '/collection/quan-short-nam',
        displayOrder: 3,
        gender: 'male'
      },
      {
        id: '4',
        name: 'QUẦN LÓT',
        imageUrl: 'https://images.unsplash.com/photo-1559583985-c80d8ad9b29f?q=80&w=3087&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D',
        link: '/collection/quan-lot-nam',
        displayOrder: 4,
        gender: 'male'
      },
      {
        id: '5',
        name: 'ĐỒ BƠI',
        imageUrl: 'https://images.unsplash.com/photo-1559583985-c80d8ad9b29f?q=80&w=3087&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D',
        link: '/collection/do-boi-nam',
        displayOrder: 5,
        gender: 'male'
      },
      {
        id: '6',
        name: 'PHỤ KIỆN',
        imageUrl: 'https://images.unsplash.com/photo-1559583985-c80d8ad9b29f?q=80&w=3087&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D',
        link: '/collection/phu-kien-nam',
        displayOrder: 6,
        gender: 'male'
      },
      // Female categories
      {
        id: '7',
        name: 'BRA & LEGGINGS',
        imageUrl: 'https://images.unsplash.com/photo-1559583985-c80d8ad9b29f?q=80&w=3087&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D',
        link: '/collection/bra-legging',
        displayOrder: 1,
        gender: 'female'
      },
      {
        id: '8',
        name: 'ÁO THỂ THAO',
        imageUrl: 'https://images.unsplash.com/photo-1559583985-c80d8ad9b29f?q=80&w=3087&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D',
        link: '/collection/ao-thun-nu',
        displayOrder: 2,
        gender: 'female'
      },
      {
        id: '9',
        name: 'QUẦN THỂ THAO',
        imageUrl: 'https://images.unsplash.com/photo-1559583985-c80d8ad9b29f?q=80&w=3087&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D',
        link: '/collection/quan-nu',
        displayOrder: 3,
        gender: 'female'
      },
      {
        id: '10',
        name: 'PHỤ KIỆN',
        imageUrl: 'https://images.unsplash.com/photo-1559583985-c80d8ad9b29f?q=80&w=3087&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D',
        link: '/collection/phu-kien-nu',
        displayOrder: 4,
        gender: 'female'
      }
    ];
    
    this.filterCategories();
  }

  filterCategories(): void {
    this.filteredCategories = this.categories
      .filter(category => category.gender === this.selectedGender)
      .sort((a, b) => a.displayOrder - b.displayOrder);
    this.isFadeIn = true; // Start fade in
  console.log(this.isFadeIn);
  }

  switchGender(gender: 'male' | 'female'): void {
    if (this.selectedGender !== gender) {
      this.selectedGender = gender;
      this.isFadeIn = false; // Start fade out
      this.filterCategories();
    }
  }
}
