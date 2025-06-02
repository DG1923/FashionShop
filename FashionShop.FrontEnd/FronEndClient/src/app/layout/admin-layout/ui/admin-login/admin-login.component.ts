import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthAdminService } from '../../services/auth-admin.service';

@Component({
  selector: 'app-admin-login',
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-login.component.html',
  styleUrl: './admin-login.component.css'
})
export class AdminLoginComponent {
email: string = '';
  password: string = '';
  error: string = '';
  isLoading: boolean = false;

  constructor(
    private authAdminService: AuthAdminService,
    private router: Router
  ) {}

  onSubmit() {
    if (!this.email || !this.password) return;

    this.isLoading = true;
    this.error = '';

    this.authAdminService.login(this.email, this.password).subscribe({
      next: (success) => {
        if (success) {
          this.router.navigate(['/admin/dashboard']);
        } else {
          this.error = 'Invalid credentials';
        }
        this.isLoading = false;
      },
      error: (err) => {
        this.error = 'An error occurred during login';
        this.isLoading = false;
      }
    });
  }
}
