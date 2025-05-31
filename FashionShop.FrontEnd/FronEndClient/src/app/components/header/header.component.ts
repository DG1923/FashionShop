import { CommonModule } from '@angular/common';
import { Component, HostListener, OnInit, OnDestroy } from '@angular/core';
import { debounceTime, Observable, Subscription } from 'rxjs';
import { CartService } from '../../services/cart.service';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { LoginRequest } from '../../models/auth.model'; 
import { CategoryService } from '../../services/category.service';
import { Category } from '../../models/category.model';

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
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  standalone: true, 
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent implements OnInit, OnDestroy {
  cartCount: number = 0;
  cartCount$: Observable<number>;
  private cartSubscription?: Subscription;
  private authSubscription?: Subscription;

  // Thêm biến để quản lý trạng thái logout
  isLoggingOut: boolean = false;

  async logout() {
    try {
      // Bắt đầu quá trình logout
      this.isLoggingOut = true;
      
      // Thêm delay ngắn để hiển thị hiệu ứng loading
      await new Promise(resolve => setTimeout(resolve, 800));
      
      // Thực hiện logout
      this.authService.logout();
      this.isLogginng = false;
      this.cartService.clearCart();
      
      // Navigate về trang chủ
      await this.router.navigate(['/']);
      
      // Reload trang sau khi navigate
      window.location.reload();
      
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      this.isLoggingOut = false;
    }
  }

  isMobileMenuOpen = false;
  isLogginng: boolean = false; 
  userName: string = '';

  toggleMobileMenu(): void {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
  }

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
  
  subMenuCategories = [];
  
  ngOnInit(): void {
    this.getCategories();
    this.initializeCartCount();
    
    // Subscribe to auth changes để update cart khi login/logout
    this.authSubscription = this.authService.authStateChanged$.subscribe((isLoggedIn) => {
      this.isLogginng = isLoggedIn;
      this.userName = this.authService.getUserName() || '';
      
      if (isLoggedIn) {
        this.updateCartCount();
      } else {
        this.cartService.clearCart();
      }
    });
  }

  ngOnDestroy(): void {
    // Cleanup subscriptions
    if (this.cartSubscription) {
      this.cartSubscription.unsubscribe();
    }
    if (this.authSubscription) {
      this.authSubscription.unsubscribe();
    }
  }

  initializeCartCount(): void {
    // Subscribe to cart count changes
    this.cartCount$ = this.cartService.getCartCount$();
    this.cartSubscription = this.cartCount$.subscribe(count => {
      this.cartCount = count;
    });

    // Load initial cart count if user is logged in
    if (this.isLogginng) {
      this.updateCartCount();
    }
  }

  updateCartCount(): void {
    if (this.isLogginng) {
      this.cartService.updateCartCount();
    }
  }

  constructor(
    private authService: AuthService,
    private router: Router,
    private categoryService: CategoryService, 
    private cartService: CartService
  ) {
    this.isLogginng = this.authService.isLoggedIn;
    this.userName = this.authService.getUserName() || '';
    this.cartCount$ = this.cartService.getCartCount$();
  }

  getCategories() {
    Promise.all([
      this.LoadCategory(this.CATEGORY_NAM_ID),
      this.LoadCategory(this.CATEGORY_NU_ID)
    ]).then(() => {
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
            link: `${category.id}`,
            columns: category.subCategory?.map((subCat: Category) => ({
              title: subCat.name,
              link: `${subCat.id}`,
              items: subCat.subCategory?.map((subSubCat) => ({
                title: subSubCat.name,
                link: `${subSubCat.id}`
              })) || [],
              image: {
                src: subCat.imageUrl,
                alt: subCat.name,
                caption: subCat.name,
                link: `${subCat.id}`
              }
            })) || [],
            isHovered: false
          }];
          this.MegaMenu.push(...temp);
          resolve();
        },
        error: () => resolve()
      });
    });
  }
  
  onMenuItemHover(index: number): void {
    this.menuItems.forEach((item, i) => {
      item.isHovered = i === index;
    });
  }
  
  onMenuLeave(): void {
    this.menuItems.forEach(item => item.isHovered = false);
  }
  
  @HostListener('document:click', ['$event'])
  clickOutside(event: Event): void {
    const headerElement = document.querySelector('.header-menu');
    if (headerElement && !headerElement.contains(event.target as Node)) {
      this.onMenuLeave();
    }
  }
}