import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ActivatedRoute, convertToParamMap } from '@angular/router';
import { NO_ERRORS_SCHEMA } from '@angular/core';

import { PlayerComponent } from './player.component';

describe('PlayerComponent', () => {
	let component: PlayerComponent;
	let fixture: ComponentFixture<PlayerComponent>;

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			declarations: [PlayerComponent],
			imports: [HttpClientTestingModule],
			providers: [
				{
					provide: ActivatedRoute,
					useValue: {
						snapshot: {
							paramMap: convertToParamMap({
								playerId: '76561198131670413',
							}),
						},
					},
				},
			],
			// The template composes child components declared in PlayerModule.
			schemas: [NO_ERRORS_SCHEMA],
		}).compileComponents();

		fixture = TestBed.createComponent(PlayerComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});

	it('keeps the steam id as a string so 64-bit ids do not lose precision', () => {
		expect(component.playerId).toBe('76561198131670413');
	});
});
