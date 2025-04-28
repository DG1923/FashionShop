import { Component } from '@angular/core';
import { HeaderComponent } from "../components/header/header.component";
import { SliderComponent } from "../slider/slider.component";
import { CategoryComponent } from "../components/category/category.component";
import { ListProductsComponent } from "../components/list-products/list-products.component";
import { CollectionComponent } from "../collection/collection.component";
import { SingleProductComponent } from "../components/single-product/single-product.component";
import { FooterComponent } from "../components/footer/footer.component";

@Component({
  selector: 'app-home',
  imports: [HeaderComponent, SliderComponent, CategoryComponent, ListProductsComponent, CollectionComponent, SingleProductComponent, FooterComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  standalone: true, 
})
export class HomeComponent {

}
