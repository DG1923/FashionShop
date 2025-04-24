import { CommonModule } from '@angular/common';
import { Component, HostListener } from '@angular/core';
import { debounceTime, Observable } from 'rxjs';
import { CartService } from '../services/cart.service';


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
  imports: [CommonModule], // Import CommonModule for ngIf and ngFor directives
  standalone: true, 
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  isMobileMenuOpen = false;

  toggleMobileMenu(): void {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
  }
  // cartCount$: Observable<number>;
  
  menuItems: MenuItem[] = [
    {
      title: 'NAM',
      link: '/nam',
      isHovered: false,
      columns: [
        {
          title: 'TẤT CẢ SẢN PHẨM',
          link: '/tat-ca-san-pham',
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
          title: 'ÁO NAM',
          link: '/ao-nam',
          items: [
            { title: 'Áo Tanktop', link: '/ao-tanktop' },
            { title: 'Áo Thun', link: '/ao-thun' },
            { title: 'Áo Thể Thao', link: '/ao-the-thao' },
            { title: 'Áo Polo', link: '/ao-polo' },
            { title: 'Áo Sơ Mi', link: '/ao-so-mi' },
            { title: 'Áo Dài Tay', link: '/ao-dai-tay' },
            { title: 'Áo Khoác', link: '/ao-khoac' }
          ]
        },
        {
          title: 'QUẦN NAM',
          link: '/quan-nam',
          items: [
            { title: 'Quần Short', link: '/quan-short' },
            { title: 'Quần Jogger', link: '/quan-jogger' },
            { title: 'Quần Thể Thao', link: '/quan-the-thao' },
            { title: 'Quần Dài', link: '/quan-dai' },
            { title: 'Quần Jean', link: '/quan-jean' },
            { title: 'Quần Bơi', link: '/quan-boi' }
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
      link: '/nu',
      isHovered: false,
      columns: [
        // Similar structure for women's items
        {
          title: 'TẤT CẢ SẢN PHẨM NỮ',
          link: '/tat-ca-san-pham-nu',
          items: [
            { title: 'Sản phẩm mới', link: '/san-pham-moi-nu' },
            { title: 'Bán chạy nhất', link: '/ban-chay-nhat-nu' }
          ]
        }
      ]
    },
    {
      title: 'THỂ THAO',
      link: '/the-thao',
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
          link: '/the-thao-nu',
          items: [
            { title: 'Áo thể thao nữ', link: '/ao-the-thao-nu' },
            { title: 'Quần thể thao nữ', link: '/quan-the-thao-nu' }
          ]
        }
      ]
    },
    {
      title: 'CARE & SHARE',
      link: '/care-share',
      isHovered: false,
      columns: [
        {
          title: 'CARE & SHARE',
          link: '/care-share',
          items: [
            { title: 'Sản phẩm Care & Share', link: '/san-pham-care-share' }
          ]
        }
      ]
    }
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
