import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PlayersComponent } from '../players/players.component';

@Component({
	selector: 'app-player',
	templateUrl: './player.component.html',
	styleUrls: ['./player.component.scss'],
})
export class PlayerComponent implements OnInit {
	playerId: number | undefined;

	constructor(private route: ActivatedRoute) {}
	ngOnInit(): void {
		const routeParams = this.route.snapshot.paramMap;
		this.playerId = Number(routeParams.get('playerId'));
	}
}
