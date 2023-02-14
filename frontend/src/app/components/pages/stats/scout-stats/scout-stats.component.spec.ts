import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScoutStatsComponent } from './scout-stats.component';

describe('ScoutStatsComponent', () => {
  let component: ScoutStatsComponent;
  let fixture: ComponentFixture<ScoutStatsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ScoutStatsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ScoutStatsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
