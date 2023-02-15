import { Component, Input } from '@angular/core';

@Component({
	selector: 'app-stats-header',
	templateUrl: './stats-header.component.html',
	styleUrls: ['./stats-header.component.scss'],
})
export class StatsHeaderComponent {
	@Input() title!: string;
	@Input() imagePath!: string;
}
