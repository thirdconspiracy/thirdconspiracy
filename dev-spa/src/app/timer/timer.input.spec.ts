import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TimerInput } from './timer.input';

describe('TimerInput', () => {
  let component: TimerInput;
  let fixture: ComponentFixture<TimerInput>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TimerInput]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TimerInput);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
