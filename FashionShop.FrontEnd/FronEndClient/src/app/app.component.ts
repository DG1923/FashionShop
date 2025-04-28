import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './components/header/header.component';
import { CommonModule } from '@angular/common';
import { SliderComponent } from "./slider/slider.component";
import { CategoryComponent } from "./components/category/category.component";
import { ProductCardComponent } from "./components/product-card/product-card.component";
import { FooterComponent } from "./components/footer/footer.component";
import { CollectionComponent } from "./collection/collection.component";
import { SingleProductComponent } from "./components/single-product/single-product.component";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule],
  standalone: true,
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'FronEndClient';
  
}
