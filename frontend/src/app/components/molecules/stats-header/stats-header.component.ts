import { Component, Input } from '@angular/core';

@Component({
	selector: 'app-stats-header',
	templateUrl: './stats-header.component.html',
	styleUrls: ['./stats-header.component.scss'],
})
export class StatsHeaderComponent {
	@Input() title!: string;
	@Input() imagePath!: string;
	/** True for the Steam-icon fallback used when a player has no avatar — see the
	 * component's own stylesheet for why that specific image needs a backdrop. */
	@Input() isPlaceholder = false;
}
