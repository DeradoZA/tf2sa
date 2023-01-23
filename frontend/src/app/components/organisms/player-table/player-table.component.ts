import { Component, Input } from '@angular/core';
import { Player } from 'src/app/models/player';

@Component({
	selector: 'app-player-table',
	templateUrl: './player-table.component.html',
	styleUrls: ['./player-table.component.scss'],
})
export class PlayerTableComponent {
	@Input() players!: Player[];
	displayedColumns: string[] = ['name', 'steamId'];
}
