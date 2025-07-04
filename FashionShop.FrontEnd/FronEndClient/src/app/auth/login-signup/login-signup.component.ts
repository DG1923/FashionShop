import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { LoginRequest } from '../../models/auth.model';
@Component({
  selector: 'app-login-signup',
  templateUrl: './login-signup.component.html',
  imports: [CommonModule,FormsModule,ReactiveFormsModule],  
  standalone: true, 
})
export class LoginSignupComponent implements OnInit {
  loginForm: FormGroup;
isLoading: boolean = false;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private authService: AuthService  
  ) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  ngOnInit(): void {}

  onSubmit() {
    if (this.loginForm.valid) {
      this.isLoading = true; // Set loading state
      const loginRequest:LoginRequest ={
         email:this.loginForm.value.username,
          password:this.loginForm.value.password
      }
      console.log('Login request:', loginRequest.email,' ',loginRequest.password);
      this.authService.login(loginRequest).subscribe({
        next: (response) => {
          if (response.success) {
            this.router.navigate(['/']);
          } else {
            console.error('Login failed:', response.message);
          }
        
      }, error: (error) => {
          console.error('Login error:', error);
      this.isLoading = false; // Reset loading state

          alert("Đăng nhập không thành công, vui lòng kiểm tra lại thông tin đăng nhập của bạn.");
          
        }});
      let token = this.authService.getToken();
      console.log(token); 
    }
  }
}
