import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent {
  menuItems = [
    { path: '/admin/dashboard', icon: 'grid', label: 'Dashboard' },
    { path: '/admin/products', icon: 'package', label: 'Quản lý sản phẩm' },
    { path: '/admin/orders', icon: 'shopping-cart', label: 'Quản lý đơn hàng' },
    { path: '/admin/users', icon: 'users', label: 'Quản lý người dùng' }
  ];
}