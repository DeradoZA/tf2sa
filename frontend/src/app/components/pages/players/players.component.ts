import { Component, OnInit } from '@angular/core';
import { Player } from 'src/app/models/player';
import { PlayersService } from 'src/app/services/players/players.service';

@Component({
	selector: 'app-players',
	templateUrl: './players.component.html',
	styleUrls: ['./players.component.scss'],
})
export class PlayersComponent implements OnInit {
	public static readonly PATH: string = 'players';

	constructor(private playersService: PlayersService) {}
	players: Player[] | undefined;

	showPlayers() {
		this.playersService.getPlayers().subscribe((data: Player[]) => {
			console.log(`showPlayers`);
			this.players = data;
		});
	}

	ngOnInit(): void {
		this.showPlayers();
	}
}
