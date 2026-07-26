import { Component, Input } from '@angular/core';
import { PlayerSynergy } from 'src/app/services/players/getPlayerDetailResult';

@Component({
	selector: 'app-player-synergy-table',
	templateUrl: './player-synergy-table.component.html',
	styleUrls: ['./player-synergy-table.component.scss'],
})
export class PlayerSynergyTableComponent {
	@Input() title = '';
	@Input() rows: PlayerSynergy[] = [];

	readonly displayedColumns: string[] = [
		'profilePicture',
		'playerName',
		'numberOfGames',
		'winPercentage',
	];
}
