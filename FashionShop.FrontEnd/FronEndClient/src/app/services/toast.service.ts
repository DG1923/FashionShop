import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface Toast {
  id: number;
  message: string;
  type: 'success' | 'error';
}

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  private toastSubject = new BehaviorSubject<Toast[]>([]);
  toasts$ = this.toastSubject.asObservable();

  success(message: string) {
    this.show(message, 'success');
  }

  error(message: string) {
    this.show(message, 'error');
  }

  private show(message: string, type: 'success' | 'error') {
    const toast: Toast = {
      id: Date.now(),
      message,
      type
    };

    const currentToasts = this.toastSubject.value;
    this.toastSubject.next([...currentToasts, toast]);

    // Auto remove after 3 seconds
    setTimeout(() => {
      const toasts = this.toastSubject.value;
      this.toastSubject.next(toasts.filter(t => t.id !== toast.id));
    }, 3000);
  }
}