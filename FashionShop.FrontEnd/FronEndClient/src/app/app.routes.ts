import { Routes } from '@angular/router';
import { LoginSignupComponent } from './auth/login-signup/login-signup.component';
import { HomeComponent } from './home/home.component';

export const routes: Routes = [
    {
        path: '',
        component: HomeComponent,
        pathMatch: 'full',
        
    },
    {
        path:'login',
        component: LoginSignupComponent,
        
    },
    {
        path:'register',    
        component: LoginSignupComponent,
    },
    {
        path:'home',
        component: HomeComponent,
        
    }

];
