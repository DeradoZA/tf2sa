import { Component } from '@angular/core';

@Component({
	selector: 'app-players',
	templateUrl: './players.component.html',
	styleUrls: ['./players.component.scss'],
})
export class PlayersComponent {
	public static readonly PATH: string = 'players';
}
