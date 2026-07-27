import { Component, Input } from '@angular/core';
import { PlayerWeaponSplit } from 'src/app/services/players/getPlayerDetailResult';

@Component({
	selector: 'app-player-weapon-splits-table',
	templateUrl: './player-weapon-splits-table.component.html',
	styleUrls: ['./player-weapon-splits-table.component.scss'],
})
export class PlayerWeaponSplitsTableComponent {
	@Input() weaponSplits: PlayerWeaponSplit[] = [];

	readonly displayedColumns: string[] = [
		'weaponName',
		'numberOfGames',
		'totalKills',
		'totalDamage',
		'accuracy',
	];
}
