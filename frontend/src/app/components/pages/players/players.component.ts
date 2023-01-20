import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { Player } from 'src/app/models/player';
import { PlayersService } from 'src/app/services/players/players.service';

@Component({
	selector: 'app-players',
	templateUrl: './players.component.html',
	styleUrls: ['./players.component.scss'],
})
export class PlayersComponent implements OnInit, OnDestroy {
	public static readonly PATH: string = 'players';

	constructor(private playersService: PlayersService) {}
	subscription: Subscription | undefined;
	players: Player[] | undefined;
	// TOOO split out error banner into separate component and apply correct themeing
	// milestone: 5
	errorMessage: string | undefined;

	ngOnInit(): void {
		this.subscription = this.playersService.getPlayers().subscribe({
			next: (players) => (this.players = players),
			error: (error) => (this.errorMessage = error),
			complete: () => console.log('complete'),
		});
	}

	ngOnDestroy(): void {
		this.subscription?.unsubscribe();
	}
}
