import { CommonModule } from '@angular/common';
import { Component, HostListener } from '@angular/core';
import { debounceTime, Observable } from 'rxjs';
import { CartService } from '../../services/cart.service';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { LoginRequest } from '../../models/auth.model'; 
import { CategoryService } from '../../services/category.service';
import { Category } from '../../models/category.model';
import{OnInit} from '@angular/core';
interface MenuItem {
  title: string;
  link: string;
  columns?: MenuColumn[];
  isHovered?: boolean;
}

interface MenuColumn {
  title?: string;
  link?: string;
  items: MenuSubItem[];
  image?: {
    src: string;
    alt: string;
    caption: string;
    link: string;
  };
}

interface MenuSubItem {
  title: string;
  link: string;
}


@Component({
  selector: 'app-header',
  imports: [CommonModule,RouterOutlet, RouterLink,RouterLinkActive], // Import CommonModule for ngIf and ngFor directives
  standalone: true, 
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent implements OnInit {
  logout() {
    this.authService.logout();
    this.isLogginng = false;
    this.router.navigate(['/']);  
  }
  isMobileMenuOpen = false;
  isLogginng:boolean= false; 
  userName : string = '';
  toggleMobileMenu(): void {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
  }
  // cartCount$: Observable<number>;
  CATEGORY_NAM_ID = '819fe665-9c1a-43a3-bcab-8f45c7b128a4';
  CATEGORY_NU_ID = '731609e6-e745-4b7b-9d19-9029b0006998';
  MegaMenu: MenuItem[] = [];

  menuItems: MenuItem[] = [
    {
      title: 'NAM',
      link: '/collection',
      isHovered: false,
      columns: [
        {
          title: 'TẤT CẢ SẢN PHẨM',
          link: '/collection',
          items: [
            { title: 'Sản phẩm mới', link: '/san-pham-moi' },
            { title: 'Bán chạy nhất', link: '/ban-chay-nhat' },
            { title: 'ECC Collection', link: '/ecc-collection' },
            { title: 'Excool Collection', link: '/excool-collection' },
            { title: 'Copper Denim', link: '/copper-denim' },
            { title: 'Promax', link: '/promax' }
          ]
        },
        {
          image: {
            src: '/assets/images/quan-jeans-nam.jpg',
            alt: 'Quần Jeans Nam siêu nhẹ',
            caption: 'Quần Jeans Nam siêu nhẹ',
            link: '/quan-jeans-nam'
          },
          items: []
        }
      ]
    },
    {
      title: 'NỮ',
      link: '/collection',
      isHovered: false,
      columns: [
        // Similar structure for women's items
        {
          title: 'TẤT CẢ SẢN PHẨM NỮ',
          link: '/collection',
          items: [
            { title: 'Sản phẩm mới', link: '/san-pham-moi-nu' },
            { title: 'Bán chạy nhất', link: '/ban-chay-nhat-nu' }
          ]
        }
      ]
    },
    {
      title: 'THỂ THAO',
      link: '/collection',
      isHovered: false,
      columns: [
        {
          title: 'THỂ THAO NAM',
          link: '/the-thao-nam',
          items: [
            { title: 'Áo thể thao nam', link: '/ao-the-thao-nam' },
            { title: 'Quần thể thao nam', link: '/quan-the-thao-nam' }
          ]
        },
        {
          title: 'THỂ THAO NỮ',
          link: '/collection',
          items: [
            { title: 'Áo thể thao nữ', link: '/ao-the-thao-nu' },
            { title: 'Quần thể thao nữ', link: '/quan-the-thao-nu' }
          ]
        }
      ]
    },
  ];
  
  subMenuCategories = [
  ];
  
  
  ngOnInit(): void {
    this.getCategories();
  }

  constructor(
    private authService: AuthService,
    private router: Router,
    private categoryService: CategoryService, 
    private cartService: CartService
  ) {
    this.isLogginng = this.authService.isLoggedIn;
    this.userName = this.authService.getUserInfo()?.name || '';
  }

  getCategories() {
    // Sử dụng Promise.all để đợi cả 2 category load xong
    Promise.all([
      this.LoadCategory(this.CATEGORY_NAM_ID),
      this.LoadCategory(this.CATEGORY_NU_ID)
    ]).then(() => {
      // Chỉ cập nhật menuItems sau khi tất cả categories đã được load
      if(this.MegaMenu.length > 0) {
        this.menuItems = [...this.MegaMenu];
        console.log('MegaMenu initialized:', this.menuItems);
      }
    });
  }

  LoadCategory(id: string): Promise<void> {
    return new Promise((resolve) => {
      this.categoryService.getSubCategoryById(id).subscribe({
        next: (category) => {
          const temp = [{
            title: category.name,
            link: `/category/${category.id}`,
            columns: category.subCategory?.map((subCat: Category) => ({
              title: subCat.name,
              link: `/category/${subCat.id}`,
              items: subCat.subCategory?.map((subSubCat) => ({
                title: subSubCat.name,
                link: `/category/${subSubCat.id}`
              })) || [],
              image: {
                src: subCat.imageUrl,
                alt: subCat.name,
                caption: subCat.name,
                link: `/category/${subCat.id}`
              }
            })) || [],
            isHovered: false
          }];
          this.MegaMenu.push(...temp);
          resolve();
        },
        error: () => resolve() // Resolve even on error to prevent hanging
      });
    });
  }
  
  // Handle hover states
  onMenuItemHover(index: number): void {
    this.menuItems.forEach((item, i) => {
      item.isHovered = i === index;
    });
  }
  
  onMenuLeave(): void {
     // Duration for the fade-out effect
    
    this.menuItems.forEach(item => item.isHovered = false);
  }
  
  // Close dropdown when clicking outside
  @HostListener('document:click', ['$event'])
  clickOutside(event: Event): void {
    const headerElement = document.querySelector('.header-menu');
    if (headerElement && !headerElement.contains(event.target as Node)) {
      this.onMenuLeave();
    }
  }
}
