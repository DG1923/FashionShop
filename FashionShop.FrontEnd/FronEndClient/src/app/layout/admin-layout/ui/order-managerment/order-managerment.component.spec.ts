import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderManagementComponent } from './order-managerment.component';

describe('OrderManagermentComponent', () => {
  let component: OrderManagementComponent;
  let fixture: ComponentFixture<OrderManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrderManagementComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OrderManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
