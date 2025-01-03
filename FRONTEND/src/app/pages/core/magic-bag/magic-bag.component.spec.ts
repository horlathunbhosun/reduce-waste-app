import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MagicBagComponent } from './magic-bag.component';

describe('MagicBagComponent', () => {
  let component: MagicBagComponent;
  let fixture: ComponentFixture<MagicBagComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MagicBagComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MagicBagComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
