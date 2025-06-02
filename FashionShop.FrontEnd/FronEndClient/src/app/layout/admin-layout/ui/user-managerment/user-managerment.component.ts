import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserAdminService, User, UpdateUserDto } from '../../services/user-admin.service';

@Component({
  selector: 'app-user-managerment',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-managerment.component.html',
  styleUrls: ['./user-managerment.component.css']
})
export class UserManagermentComponent implements OnInit {
  users: User[] = [];
  selectedUser: User | null = null;
  isLoading = false;
  error: string | null = null;
  isEditing = false;
  updateForm: UpdateUserDto = {
    userName: '',
    email: '',
    password: ''
  };

  constructor(private userService: UserAdminService) {}
  ngOnInit(): void {
    this.loadAllUsers(); 
  }
  loadAllUsers() {
    this.isLoading = true;
    this.error = null;
    
    this.userService.getAllUsers()
      .subscribe({
        next: (users) => {
          this.users = users;
          this.isLoading = false;
        },
        error: (err) => {
          console.error('Error loading users:', err);
          this.error = 'Không thể tải danh sách người dùng';
          this.isLoading = false;
        }
      });
  }
  editUser(user: User) {
    this.selectedUser = user;
    this.updateForm = {
      userName: user.userName,
      email: user.email,
      password: ''
    };
    this.isEditing = true;
  }

  cancelEdit() {
    this.selectedUser = null;
    this.isEditing = false;
    this.updateForm = {
      userName: '',
      email: '',
      password: ''
    };
  }

    // Sửa lại phương thức updateUser để reload danh sách sau khi cập nhật
  updateUser() {
    if (!this.selectedUser) return;

    this.isLoading = true;
    this.userService.updateUser(this.selectedUser.id, this.updateForm)
      .subscribe({
        next: () => {
          this.isLoading = false;
          this.cancelEdit();
          // Reload all users after update
          this.loadAllUsers();
        },
        error: (err) => {
          this.error = 'Cập nhật thất bại';
          this.isLoading = false;
        }
      });
  }

  // Sửa lại phương thức deleteUser để hiển thị thông báo tiếng Việt
  deleteUser(userId: string) {
    if (confirm('Bạn có chắc chắn muốn xóa người dùng này?')) {
      this.isLoading = true;
      this.userService.deleteUser(userId)
        .subscribe({
          next: () => {
            this.isLoading = false;
            this.users = this.users.filter(u => u.id !== userId);
          },
          error: (err) => {
            this.error = 'Xóa người dùng thất bại';
            this.isLoading = false;
          }
        });
    }
  }

  loadUser(id: string) {
    this.isLoading = true;
    this.userService.getUserById(id)
      .subscribe({
        next: (user) => {
          const index = this.users.findIndex(u => u.id === id);
          if (index !== -1) {
            this.users[index] = user;
          }
          this.isLoading = false;
        },
        error: (err) => {
          this.error = 'Failed to load user';
          this.isLoading = false;
        }
      });
  }
}