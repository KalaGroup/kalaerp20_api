import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogComponantComponent } from './dialog-componant.component';

describe('DialogComponantComponent', () => {
  let component: DialogComponantComponent;
  let fixture: ComponentFixture<DialogComponantComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DialogComponantComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogComponantComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
