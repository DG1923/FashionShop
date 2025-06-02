import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProducrManagermentComponent } from './producr-managerment.component';

describe('ProducrManagermentComponent', () => {
  let component: ProducrManagermentComponent;
  let fixture: ComponentFixture<ProducrManagermentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProducrManagermentComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProducrManagermentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
