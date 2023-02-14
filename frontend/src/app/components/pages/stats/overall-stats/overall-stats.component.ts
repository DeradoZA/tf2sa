import { Component } from '@angular/core';

@Component({
	selector: 'app-overall-stats',
	templateUrl: './overall-stats.component.html',
	styleUrls: ['./overall-stats.component.scss'],
})
export class OverallStatsComponent {
	readonly isLoaded: boolean = true;
	readonly errorMessage: string = 'Not available yet';
}
