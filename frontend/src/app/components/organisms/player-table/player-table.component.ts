import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { GetPlayersResult } from 'src/app/services/players/getPlayersResult';
import { PlayersService } from 'src/app/services/players/players.service';

@Component({
	selector: 'app-player-table',
	templateUrl: './player-table.component.html',
	styleUrls: ['./player-table.component.scss'],
})
export class PlayerTableComponent implements OnInit, OnDestroy {
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

	displayedColumns: string[] = ['name', 'steamId'];
}
