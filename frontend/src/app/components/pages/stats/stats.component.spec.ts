import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';

import { StatsComponent } from './stats.component';

describe('StatsComponent', () => {
	let component: StatsComponent;
	let fixture: ComponentFixture<StatsComponent>;

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			declarations: [StatsComponent],
			imports: [RouterTestingModule],
		}).compileComponents();

		fixture = TestBed.createComponent(StatsComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});
