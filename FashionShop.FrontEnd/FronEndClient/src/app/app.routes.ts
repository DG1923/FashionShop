import { Routes } from '@angular/router';
import { LoginSignupComponent } from './auth/login-signup/login-signup.component';
import { HomeComponent } from './layout/home/home.component';
import { RegisterComponent } from './auth/register/register.component';
import { CollectionComponent } from './collection/collection.component';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { SingleProductComponent } from './components/single-product/single-product.component';
import { CartComponent } from './components/cart/cart.component';
import { CheckoutComponent } from './components/checkout/checkout.component';
import { OrderComponent } from './components/order/order.component';
import { AdminComponent } from './layout/admin-layout/ui/admin/admin.component';
import { AdminGuard } from './guards/admin.guard';

export const routes: Routes = [
    {
        path: '',
        component: MainLayoutComponent,
        children:[
            {path: '', component: HomeComponent, pathMatch: 'full'},   
            {path: 'collection', component: CollectionComponent},
            { path: 'product/:id', component: SingleProductComponent },
            {path: 'collection/:category', component: CollectionComponent},
            {path:'cart', component: CartComponent},
            {path:'checkout', component: CheckoutComponent},
            {path:'order', component: OrderComponent},

        ]
        
    },
    {
        path: 'admin',
        children: [
            {
                path: 'login',
                loadComponent: () => import('./layout/admin-layout/ui/admin-login/admin-login.component')
                    .then(m => m.AdminLoginComponent)
            },
            {
                path: '',
                component: AdminComponent,
                canActivate: [AdminGuard],
                children: [
                    {
                        path: 'dashboard',
                        loadComponent: () => import('./layout/admin-layout/ui/dashboard/dashboard.component')
                          .then(m => m.DashboardComponent)
                      },
                      {
                        path: 'products',
                        loadComponent: () => import('./layout/admin-layout/ui/producr-managerment/producr-managerment.component')
                          .then(m => m.ProducrManagermentComponent)
                      },
                      {
                        path: 'orders',
                        loadComponent: () => import('./layout/admin-layout/ui/order-managerment/order-managerment.component')
                          .then(m => m.OrderManagementComponent)
                      },
                      {
                        path: 'users',
                        loadComponent: () => import('./layout/admin-layout/ui/user-managerment/user-managerment.component')
                          .then(m => m.UserManagermentComponent)
                      },
                      {
                        path: '',
                        redirectTo: 'dashboard',
                        pathMatch: 'full'
                      }
                ]
            }
        ]
    },
    {
        path:'login',
        component: LoginSignupComponent,
        
    },
    {
        path:'register',    
        component: RegisterComponent,
    },

];
