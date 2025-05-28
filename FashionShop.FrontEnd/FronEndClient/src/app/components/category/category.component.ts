import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CategoryService } from '../../services/category.service';
import { CategoryDisplayDto } from '../../models/category.model';
@Component({
  selector: 'app-category',
  imports: [CommonModule,RouterModule],
  standalone:true,
  templateUrl: './category.component.html',
  styleUrl: './category.component.css'
})
export class CategoryComponent implements OnInit {
  categories: CategoryDisplayDto[] = [];
  filteredCategories: CategoryDisplayDto[] = [];
  selectedGender: 'male' | 'female' = 'male';
  isFadeIn = false; 
  isLoading = false;
  errorMessage: string | null = null; 
  readonly DO_NAM_ID = '819fe665-9c1a-43a3-bcab-8f45c7b128a4';
  readonly DO_NU_ID = '731609e6-e745-4b7b-9d19-9029b0006998';
  constructor(private categoryService:CategoryService) { }

  ngOnInit(): void {
    this.isLoading = true;
    this.isFadeIn = true;
    
    // Gọi cả 2 API
    this.GetSubCategories(this.DO_NAM_ID);
    this.GetSubCategories(this.DO_NU_ID);
    
    // Tắt loading sau 300ms
    setTimeout(() => {
      this.isLoading = false;
    }, 300);
}

  GetSubCategories(id: string = this.DO_NAM_ID): void {
    this.categoryService.getSubCategoryById(id)
      .subscribe({
        next: (data) => {
          console.log("received data from service", data);
          if (data && data.subCategory) {
            // Map new categories
            const newCategories = data.subCategory.map(
              (subCategory) => ({
                id: subCategory.id,
                name: subCategory.name,
                imageUrl: subCategory.imageUrl,
                link: this.generateSlug(subCategory.name,subCategory.id),
                gender: id === this.DO_NAM_ID ? "male" as 'male' : "female" as 'female'
              })
            );
            
            // Add to existing array instead of replacing
            this.categories = [...this.categories, ...newCategories];
            
            // Filter and show categories
            this.filterCategories();
          }
        },
        error: (error) => {
          console.error('Error fetching subcategories with ID ', id, ' :', error);
          this.errorMessage = 'Không thể tải danh mục sản phẩm';
        }
      });
  }
    private generateSlug(name: string, id:string): string {
    return name
      .toLowerCase()
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .replace(/[đĐ]/g, 'd')
      .replace(/([^0-9a-z-\s])/g, '')
      .replace(/(\s+)/g, '-')
      .replace(/-+/g, '-')
      .replace(/^-+|-+$/g, '')+"--",id;
  }
  loadCategories(): void {
    // In a real application, this would be retrieved from a service
    this.categories = [
      {
        id: '1',
        name: 'ÁO THUN',
        imageUrl: 'https://images.unsplash.com/photo-1559583985-c80d8ad9b29f?w=900&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxjb2xsZWN0aW9uLXBhZ2V8MXwxMDY1OTc2fHxlbnwwfHx8fHw%3D',
        link: '/collection/ao-thun-nam',
        
        gender: 'male'
      },
      {
        id: '2',
        name: 'ÁO POLO',
        imageUrl: 'https://images.unsplash.com/photo-1502989642968-94fbdc9eace4?q=80&w=3088&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D',
        link: '/collection/ao-polo-nam',
      
        gender: 'male'
      },
      {
        id: '3',
        name: 'QUẦN SHORT',
        imageUrl: 'https://images.unsplash.com/photo-1502989642968-94fbdc9eace4?q=80&w=3088&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D',
        link: '/collection/quan-short-nam',
        
        gender: 'male'
      },
      {
        id: '4',
        name: 'QUẦN LÓT',
        imageUrl: 'https://images.unsplash.com/photo-1559583985-c80d8ad9b29f?q=80&w=3087&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D',
        link: '/collection/quan-lot-nam',
        
        gender: 'male'
      },
      {
        id: '5',
        name: 'ĐỒ BƠI',
        imageUrl: 'https://images.unsplash.com/photo-1559583985-c80d8ad9b29f?q=80&w=3087&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D',
        link: '/collection/do-boi-nam',
        
        gender: 'male'
      },
      {
        id: '6',
        name: 'PHỤ KIỆN',
        imageUrl: 'https://images.unsplash.com/photo-1559583985-c80d8ad9b29f?q=80&w=3087&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D',
        link: '/collection/phu-kien-nam',
        
        gender: 'male'
      },
      // Female categories
      {
        id: '7',
        name: 'BRA & LEGGINGS',
        imageUrl: 'https://images.unsplash.com/photo-1559583985-c80d8ad9b29f?q=80&w=3087&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D',
        link: '/collection/bra-legging',
        
        gender: 'female'
      },
      {
        id: '8',
        name: 'ÁO THỂ THAO',
        imageUrl: 'https://images.unsplash.com/photo-1559583985-c80d8ad9b29f?q=80&w=3087&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D',
        link: '/collection/ao-thun-nu',
        
        gender: 'female'
      },
      {
        id: '9',
        name: 'QUẦN THỂ THAO',
        imageUrl: 'https://images.unsplash.com/photo-1559583985-c80d8ad9b29f?q=80&w=3087&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D',
        link: '/collection/quan-nu',
        
        gender: 'female'
      },
      {
        id: '10',
        name: 'PHỤ KIỆN',
        imageUrl: 'https://images.unsplash.com/photo-1559583985-c80d8ad9b29f?q=80&w=3087&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D',
        link: '/collection/phu-kien-nu',
          
        gender: 'female'
      }
    ];
    
    this.filterCategories();
  }

  filterCategories(): void {
    this.filteredCategories = this.categories
      .filter(category => category.gender === this.selectedGender);
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
