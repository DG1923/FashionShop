import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormModule } from '@coreui/angular';
import { DashboardService, TopSellingProduct } from '../../services/dashboard.service';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-dashboard',
  imports: [FormModule, CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  totalRevenue: number = 0;
  topSellingProducts: TopSellingProduct[] = [];
  isLoading: boolean = false;
  error: string | null = null;

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.isLoading = true;
    this.error = null;

    forkJoin({
      revenue: this.dashboardService.getTotalRevenue(),
      products: this.dashboardService.getTopSellingProducts()
    }).subscribe({
      next: (data) => {
        this.totalRevenue = data.revenue;
        this.topSellingProducts = data.products;
        this.isLoading = false;
      },
      error: (err) => {
        this.error = 'Không thể tải dữ liệu dashboard';
        this.isLoading = false;
        console.error('Dashboard error:', err);
      }
    });
  }

  // Format tiền tệ VND
  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND'
    }).format(amount);
  }

  // Refresh data
  refreshData(): void {
    this.loadDashboardData();
  }

  

  // Get rank style classes
  getRankClasses(index: number): string {
    switch (index) {
      case 0: return 'bg-gradient-to-r from-yellow-400 to-yellow-600 text-white';
      case 1: return 'bg-gradient-to-r from-gray-300 to-gray-500 text-white';
      case 2: return 'bg-gradient-to-r from-amber-600 to-amber-800 text-white';
      default: return 'bg-gray-100 text-gray-600';
    }
  }

  // Get medal icon
  getMedalIcon(index: number): string {
    switch (index) {
      case 0: return '🥇';
      case 1: return '🥈';
      case 2: return '🥉';
      default: return '';
    }
  }
}
