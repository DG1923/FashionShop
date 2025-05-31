import { Component, OnInit, OnDestroy } from "@angular/core";
import { CommonModule } from "@angular/common";
import { ToastService, Toast } from "../services/toast.service";
import { Subject, takeUntil } from "rxjs";

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="fixed top-4 right-4 z-50 space-y-3 max-w-sm">
      @for(toast of toasts; track toast.id) {
        <div class="toast-container transform transition-all duration-350 ease-out"
             [class]="getToastClasses(toast)"
             (mouseenter)="pauseTimer(toast.id)"
             (mouseleave)="resumeTimer(toast.id)">
          
          <!-- Toast Content -->
          <div class="flex items-start space-x-3 p-4">
            <!-- Icon -->
            @if(toast.icon) {
              <div class="flex-shrink-0">
                <div class="toast-icon text-lg" [class]="getIconClasses(toast.type)">
                  {{ toast.icon }}
                </div>
              </div>
            }
            
            <!-- Message Content -->
            <div class="flex-1 min-w-0">
              <div class="text-sm font-medium" [class]="getTextClasses(toast.type)">
                {{ toast.message }}
              </div>
              
              <!-- Action Button -->
              @if(toast.action) {
                <button (click)="handleAction(toast)"
                        class="mt-2 text-xs font-semibold underline hover:no-underline transition-all"
                        [class]="getActionClasses(toast.type)">
                  {{ toast.action.label }}
                </button>
              }
            </div>
            
            <!-- Close Button -->
            @if(toast.dismissible) {
              <button (click)="dismiss(toast.id)"
                      class="flex-shrink-0 p-1 rounded-full transition-all duration-200 hover:scale-110"
                      [class]="getCloseButtonClasses(toast.type)">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
                </svg>
              </button>
            }
          </div>
          
          <!-- Progress Bar -->
          @if(toast.duration && toast.duration > 0 && !toast.isRemoving) {
            <div class="absolute bottom-0 left-0 right-0 h-1 bg-black bg-opacity-20 rounded-b-xl overflow-hidden">
              <div class="progress-bar h-full transition-all ease-linear"
                   [class]="getProgressBarClasses(toast.type)"
                   [style.animation-duration.ms]="toast.duration">
              </div>
            </div>
          }
        </div>
      }
      
      <!-- Clear All Button (when multiple toasts) -->
      @if(toasts.length > 1) {
        <div class="flex justify-end mt-2">
          <button (click)="dismissAll()"
                  class="text-xs text-gray-500 hover:text-gray-700 bg-white bg-opacity-90 px-3 py-1 rounded-full shadow-sm hover:shadow-md transition-all duration-200">
            Đóng tất cả
          </button>
        </div>
      }
    </div>
  `,
  styles: [`
    /* Base toast container */
    .toast-container {
      backdrop-filter: blur(10px);
      border-radius: 12px;
      box-shadow: 0 10px 25px rgba(0, 0, 0, 0.1), 0 4px 10px rgba(0, 0, 0, 0.05);
      border: 1px solid rgba(255, 255, 255, 0.2);
      max-width: 400px;
      min-width: 300px;
      position: relative;
      overflow: hidden;
    }

    /* Entrance Animation */
    .toast-enter {
      opacity: 0;
      transform: translateX(100%) scale(0.95);
    }

    .toast-visible {
      opacity: 1;
      transform: translateX(0) scale(1);
    }

    /* Exit Animation */
    .toast-exit {
      opacity: 0;
      transform: translateX(100%) scale(0.95);
      pointer-events: none;
    }

    /* Toast Type Styles - Fashion themed */
    .toast-success {
      background: linear-gradient(135deg, #10b981 0%, #059669 100%);
      color: white;
    }

    .toast-error {
      background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%);
      color: white;
    }

    .toast-warning {
      background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);
      color: white;
    }

    .toast-info {
      background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%);
      color: white;
    }

    /* Icon animations */
    .toast-icon {
      animation: iconBounce 0.6s ease-out;
    }

    @keyframes iconBounce {
      0%, 20%, 60%, 100% {
        transform: translateY(0);
      }
      40% {
        transform: translateY(-4px);
      }
      80% {
        transform: translateY(-2px);
      }
    }

    /* Progress bar animation */
    .progress-bar {
      width: 100%;
      animation-name: progressShrink;
      animation-timing-function: linear;
      animation-fill-mode: forwards;
    }

    @keyframes progressShrink {
      from {
        width: 100%;
      }
      to {
        width: 0%;
      }
    }

    .progress-success {
      background: rgba(255, 255, 255, 0.3);
    }

    .progress-error {
      background: rgba(255, 255, 255, 0.3);
    }

    .progress-warning {
      background: rgba(255, 255, 255, 0.3);
    }

    .progress-info {
      background: rgba(255, 255, 255, 0.3);
    }

    /* Hover effects */
    .toast-container:hover {
      transform: translateY(-2px);
      box-shadow: 0 15px 30px rgba(0, 0, 0, 0.15), 0 8px 15px rgba(0, 0, 0, 0.1);
    }

    .toast-container:hover .progress-bar {
      animation-play-state: paused;
    }

    /* Mobile responsive */
    @media (max-width: 640px) {
      .toast-container {
        min-width: 280px;
        max-width: calc(100vw - 2rem);
      }
    }

    /* Accessibility */
    @media (prefers-reduced-motion: reduce) {
      .toast-container,
      .toast-icon,
      .progress-bar {
        animation: none !important;
        transition: none !important;
      }
    }
  `]
})
export class ToastComponent implements OnInit, OnDestroy {
  toasts: Toast[] = [];
  private destroy$ = new Subject<void>();
  private pausedToasts = new Set<number>();

  constructor(private toastService: ToastService) {}

  ngOnInit() {
    this.toastService.toasts$
      .pipe(takeUntil(this.destroy$))
      .subscribe(toasts => {
        this.toasts = toasts;
      });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  getToastClasses(toast: Toast): string {
    let classes = `toast-container toast-${toast.type}`;
    
    if (!toast.isVisible) {
      classes += ' toast-enter';
    } else if (toast.isRemoving) {
      classes += ' toast-exit';
    } else {
      classes += ' toast-visible';
    }
    
    return classes;
  }

  getIconClasses(type: string): string {
    return `toast-icon-${type}`;
  }

  getTextClasses(type: string): string {
    return 'text-white';
  }

  getActionClasses(type: string): string {
    return 'text-white text-opacity-90 hover:text-opacity-100';
  }

  getCloseButtonClasses(type: string): string {
    return 'text-white text-opacity-70 hover:text-opacity-100 hover:bg-white hover:bg-opacity-20';
  }

  getProgressBarClasses(type: string): string {
    return `progress-${type}`;
  }

  dismiss(id: number) {
    this.toastService.dismiss(id);
  }

  dismissAll() {
    this.toastService.dismissAll();
  }

  handleAction(toast: Toast) {
    if (toast.action) {
      toast.action.handler();
      this.dismiss(toast.id);
    }
  }

  pauseTimer(id: number) {
    this.pausedToasts.add(id);
  }

  resumeTimer(id: number) {
    this.pausedToasts.delete(id);
  }
}