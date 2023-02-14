import { Component } from '@angular/core';
import { ROUTES } from 'src/app/routes';

@Component({
	selector: 'app-stats',
	templateUrl: './stats.component.html',
	styleUrls: ['./stats.component.scss'],
})
export class StatsComponent {
	readonly ROUTES = ROUTES;
}
