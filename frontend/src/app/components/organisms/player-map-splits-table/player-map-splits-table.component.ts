import { Component, Input } from '@angular/core';
import { PlayerMapSplit } from 'src/app/services/players/getPlayerDetailResult';

@Component({
	selector: 'app-player-map-splits-table',
	templateUrl: './player-map-splits-table.component.html',
	styleUrls: ['./player-map-splits-table.component.scss'],
})
export class PlayerMapSplitsTableComponent {
	@Input() mapSplits: PlayerMapSplit[] = [];

	readonly displayedColumns: string[] = [
		'map',
		'numberOfGames',
		'wins',
		'winPercentage',
		'averageDpm',
		'averageKills',
		'averageDeaths',
	];
}
