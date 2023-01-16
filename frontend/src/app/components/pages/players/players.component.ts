import { Component } from '@angular/core';

@Component({
	selector: 'app-players',
	templateUrl: './players.component.html',
	styleUrls: ['./players.component.css'],
})
export class PlayersComponent {
	public static readonly PATH: string = 'players';
}
