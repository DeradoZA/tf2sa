import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DemomanStatsComponent } from './demoman-stats.component';

describe('DemomanStatsComponent', () => {
  let component: DemomanStatsComponent;
  let fixture: ComponentFixture<DemomanStatsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DemomanStatsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DemomanStatsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
