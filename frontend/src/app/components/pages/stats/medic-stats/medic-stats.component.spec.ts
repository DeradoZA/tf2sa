import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicStatsComponent } from './medic-stats.component';

describe('MedicStatsComponent', () => {
  let component: MedicStatsComponent;
  let fixture: ComponentFixture<MedicStatsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MedicStatsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MedicStatsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
