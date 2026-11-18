import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TimerOutputAlarm } from './timer.output.alarm';

describe('TimerOutputAlarm', () => {
  let component: TimerOutputAlarm;
  let fixture: ComponentFixture<TimerOutputAlarm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TimerOutputAlarm]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TimerOutputAlarm);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
