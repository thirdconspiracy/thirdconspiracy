import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TimerOutputCountdown } from './timer.output.countdown';

describe('TimerOutputCountdown', () => {
  let component: TimerOutputCountdown;
  let fixture: ComponentFixture<TimerOutputCountdown>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TimerOutputCountdown]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TimerOutputCountdown);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
