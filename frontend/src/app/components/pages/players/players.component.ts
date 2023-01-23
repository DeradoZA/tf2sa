import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { Player } from 'src/app/models/player';
import { GetPlayersResult } from 'src/app/services/players/getPlayersResult';
import { PlayersService } from 'src/app/services/players/players.service';

@Component({
	selector: 'app-players',
	templateUrl: './players.component.html',
	styleUrls: ['./players.component.scss'],
})
export class PlayersComponent implements OnInit, OnDestroy {
	public static readonly PATH: string = 'players';

	constructor(private playersService: PlayersService) {}
	isLoaded: boolean = false;
	subscription: Subscription | undefined;
	playersResult: GetPlayersResult | undefined;
	errorMessage: string | undefined;

	ngOnInit(): void {
		this.subscription = this.playersService.getPlayers().subscribe({
			next: (players) => {
				console.log(players);
				this.playersResult = players;
				this.isLoaded = true;
			},
			error: (error) => {
				this.errorMessage = error;
				this.isLoaded = true;
			},
		});
	}
	ngOnDestroy(): void {
		this.subscription?.unsubscribe();
	}
}
