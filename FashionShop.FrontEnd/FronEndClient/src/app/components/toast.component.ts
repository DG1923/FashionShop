import { Component } from "@angular/core";
import { CommonModule } from "@angular/common";
import { ToastService } from "../services/toast.service";

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="fixed top-4 right-4 z-50 space-y-2">
      @for(toast of toastService.toasts$ | async; track toast.id) {
        <div class="p-4 rounded-lg shadow-lg text-white animate-fade-in"
             [class.bg-green-500]="toast.type === 'success'"
             [class.bg-red-500]="toast.type === 'error'">
          {{ toast.message }}
        </div>
      }
    </div>
  `,
  styles: [`
    @keyframes fadeIn {
      from { opacity: 0; transform: translateY(-100%); }
      to { opacity: 1; transform: translateY(0); }
    }
    .animate-fade-in {
      animation: fadeIn 0.3s ease-in-out;
    }
  `]
})
export class ToastComponent {
  constructor(public toastService: ToastService) {}
}