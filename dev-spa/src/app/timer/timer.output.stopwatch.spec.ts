import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TimerOutputStopwatch } from './timer.output.stopwatch';

describe('TimerOutputStopwatch', () => {
  let component: TimerOutputStopwatch;
  let fixture: ComponentFixture<TimerOutputStopwatch>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TimerOutputStopwatch]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TimerOutputStopwatch);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
