import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserManagermentComponent } from './user-managerment.component';

describe('UserManagermentComponent', () => {
  let component: UserManagermentComponent;
  let fixture: ComponentFixture<UserManagermentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserManagermentComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserManagermentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
