import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PlayersComponent } from '../players/players.component';

@Component({
	selector: 'app-player',
	templateUrl: './player.component.html',
	styleUrls: ['./player.component.css'],
})
export class PlayerComponent implements OnInit {
	public static readonly PATH: string = `${PlayersComponent.PATH}/:playerId`;
	playerId: number | undefined;

	constructor(private route: ActivatedRoute) {}
	ngOnInit(): void {
		const routeParams = this.route.snapshot.paramMap;
		this.playerId = Number(routeParams.get('playerId'));
	}
}
