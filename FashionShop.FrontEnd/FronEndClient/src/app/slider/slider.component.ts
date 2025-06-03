import { CommonModule } from '@angular/common';
import { Component, ElementRef, Input, ViewChild,OnInit } from '@angular/core';

import { RouterLink } from '@angular/router';
import {
  CarouselCaptionComponent,
  CarouselComponent,
  CarouselControlComponent,
  CarouselIndicatorsComponent,
  CarouselInnerComponent,
  CarouselItemComponent
} from '@coreui/angular';

export interface BannerSlide {
  id: number;
  imageUrl: string;
  link: string;
  altText?: string;
  openInNewTab?: boolean;
  trackingValue?: string;
}

@Component({
  selector: 'app-slider',
  standalone: true, 
  imports: [CommonModule,CarouselComponent,
    CarouselIndicatorsComponent,
    CarouselInnerComponent,
    CarouselItemComponent,
    CarouselCaptionComponent,
    CarouselControlComponent,RouterLink],
  templateUrl: './slider.component.html',
  styleUrl: './slider.component.css'
})
export class SliderComponent implements OnInit {
  slides: any[] = new Array(3).fill({ id: -1, src: '', title: '', subtitle: '' });

  ngOnInit(): void {
    this.slides[0] = {
      id: 0,
      src: 'https://www.skyweaver.net/images/media/wallpapers/wallpaper1.jpg',
      title: 'Quần áo',
      subtitle: ''
    };
    this.slides[1] = {
      id: 1,
      src: 'https://www.skyweaver.net/images/media/wallpapers/wallpaper2.jpg',
      title: 'Thời trang',
      subtitle: ''
    };
    this.slides[2] = {
      id: 2,
      src: 'https://www.skyweaver.net/images/media/wallpapers/wallpaper3.jpg',
      title: 'Thời trang tối giản',
      subtitle: ''
    };
  }
}
