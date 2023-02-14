import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SoldierStatsComponent } from './soldier-stats.component';

describe('SoldierStatsComponent', () => {
  let component: SoldierStatsComponent;
  let fixture: ComponentFixture<SoldierStatsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SoldierStatsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SoldierStatsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
