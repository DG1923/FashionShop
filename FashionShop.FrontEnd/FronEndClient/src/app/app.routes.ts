import { Routes } from '@angular/router';
import { LoginSignupComponent } from './auth/login-signup/login-signup.component';
import { HomeComponent } from './layout/home/home.component';
import { RegisterComponent } from './auth/register/register.component';
import { CollectionComponent } from './collection/collection.component';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { single } from 'rxjs';
import { SingleProductComponent } from './components/single-product/single-product.component';

export const routes: Routes = [
    {
        path: '',
        component: MainLayoutComponent,
        children:[
            {path: '', component: HomeComponent, pathMatch: 'full'},   
            {path: 'collection', component: CollectionComponent},
            {path:'single-product',component:SingleProductComponent},
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
