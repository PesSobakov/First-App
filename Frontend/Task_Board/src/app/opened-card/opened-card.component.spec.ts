import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OpenedCardComponent } from './opened-card.component';

describe('OpenedCardComponent', () => {
  let component: OpenedCardComponent;
  let fixture: ComponentFixture<OpenedCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OpenedCardComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(OpenedCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
