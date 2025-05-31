// register.component.ts
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, HttpClientModule],
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  isLoading = false;
  loadingText = 'Creating your account...';
  
  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    }, {
      validator: this.passwordMatchValidator
    });
  }

  ngOnInit(): void {}

  passwordMatchValidator(g: FormGroup) {
    return g.get('password')?.value === g.get('confirmPassword')?.value
      ? null : {'mismatch': true};
  }

  onSubmit() {
    if (this.registerForm.valid && !this.isLoading) {
      this.isLoading = true;
      this.loadingText = 'Creating your account...';
      
      const registerData = {
        userName: this.registerForm.value.username,
        email: this.registerForm.value.email,
        password: this.registerForm.value.password
      };

      this.authService.register(registerData).subscribe({
        next: (response) => {
          if (response.success) {
            this.loadingText = 'Account created successfully!';
            // Small delay to show success message before navigation
            setTimeout(() => {
              this.isLoading = false;
              this.router.navigate(['/']);
            }, 800);
          } else {
            this.isLoading = false;
          }
        },
        error: (error) => {
          console.error('Registration failed:', error);
          this.loadingText = 'Registration failed. Please try again.';
          setTimeout(() => {
            this.isLoading = false;
          }, 1500);
        }
      });
    }
  }
}