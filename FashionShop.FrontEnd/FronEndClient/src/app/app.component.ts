import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './header/header.component';
import { CommonModule } from '@angular/common';
import { SliderComponent } from "./slider/slider.component";
import { CategoryComponent } from "./category/category.component";
import { ListProductsComponent } from "./list-products/list-products.component";
import { ProductCardComponent } from "./product-card/product-card.component";
import { FooterComponent } from "./footer/footer.component";
import { CollectionComponent } from "./collection/collection.component";
import { SingleProductComponent } from "./single-product/single-product.component";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent, CommonModule, SliderComponent, CategoryComponent, ListProductsComponent, ProductCardComponent, FooterComponent, CollectionComponent, SingleProductComponent],
  standalone: true,
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'FronEndClient';
  
}
