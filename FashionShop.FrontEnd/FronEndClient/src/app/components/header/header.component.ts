import { CommonModule } from '@angular/common';
import { Component, HostListener } from '@angular/core';
import { debounceTime, Observable } from 'rxjs';
import { CartService } from '../../services/cart.service';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { LoginRequest } from '../../models/auth.model'; 

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
export class HeaderComponent {
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
    { title: 'THEO NHU CẦU', link: '/theo-nhu-cau' },
    { title: 'ĐỒ LÓT', link: '/do-lot' },
    { title: 'ĐỒ THỂ THAO', link: '/do-the-thao' },
    { title: 'MẶC HÀNG NGÀY', link: '/mac-hang-ngay' }
  ];
  
  // constructor(private cartService: CartService) {
  //   // Use observable for cart count to avoid repeated API calls
  //   this.cartCount$ = this.cartService.getCartCount().pipe(
  //     debounceTime(300) // Debounce to prevent rapid UI updates
  //   );
  // }
  
  ngOnInit(): void {
    // Initialization code if needed
  }
  /**
   *
   */
  constructor(private authService:AuthService,private router:Router) {
   this.isLogginng = this.authService.isLoggedIn;
   this.userName = this.authService.getUserInfo()?.name || ''; // Default to empty string if userName is not available  
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
