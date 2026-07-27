import { Component, Input } from '@angular/core';
import { PlayerClassSplit } from 'src/app/services/players/getPlayerDetailResult';

// Class ids as used throughout the stats pipeline (see Leaderboards.sql).
const CLASS_NAMES: Record<number, string> = {
	1: 'Scout',
	2: 'Soldier',
	3: 'Pyro',
	4: 'Demoman',
	5: 'Heavy',
	6: 'Engineer',
	7: 'Medic',
	8: 'Sniper',
	9: 'Spy',
};

@Component({
	selector: 'app-player-class-splits-table',
	templateUrl: './player-class-splits-table.component.html',
	styleUrls: ['./player-class-splits-table.component.scss'],
})
export class PlayerClassSplitsTableComponent {
	@Input() classSplits: PlayerClassSplit[] = [];

	readonly displayedColumns: string[] = [
		'className',
		'numberOfGames',
		'wins',
		'averageDpm',
		'averageKills',
		'averageAssists',
		'averageDeaths',
	];

	className(classId: number): string {
		return CLASS_NAMES[classId] ?? `Class ${classId}`;
	}
}
