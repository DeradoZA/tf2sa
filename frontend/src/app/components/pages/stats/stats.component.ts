import { Component } from '@angular/core';

@Component({
	selector: 'app-stats',
	templateUrl: './stats.component.html',
	styleUrls: ['./stats.component.scss'],
})
export class StatsComponent {
	public static readonly PATH: string = 'stats';
}
