import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SupplierdetailspopupComponent } from './supplierdetailspopup.component';

describe('SupplierdetailspopupComponent', () => {
  let component: SupplierdetailspopupComponent;
  let fixture: ComponentFixture<SupplierdetailspopupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SupplierdetailspopupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SupplierdetailspopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
