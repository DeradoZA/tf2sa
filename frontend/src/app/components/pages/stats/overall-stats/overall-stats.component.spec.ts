import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OverallStatsComponent } from './overall-stats.component';

describe('OverallStatsComponent', () => {
  let component: OverallStatsComponent;
  let fixture: ComponentFixture<OverallStatsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OverallStatsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OverallStatsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
