import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { map, merge, startWith, switchMap } from 'rxjs';
import { GetPlayersResult } from 'src/app/services/players/getPlayersResult';
import { PlayersService } from 'src/app/services/players/players.service';

@Component({
	selector: 'app-player-table',
	templateUrl: './player-table.component.html',
	styleUrls: ['./player-table.component.scss'],
})
export class PlayerTableComponent implements AfterViewInit {
	readonly displayedColumns: string[] = ['name', 'steamId'];
	constructor(private playersService: PlayersService) {}
	isLoaded: boolean = false;
	playersResult: GetPlayersResult = {
		totalResults: 0,
		count: 0,
		offset: 0,
		players: [],
		sortBy: '',
		filterString: '',
	};
	errorMessage: string | undefined;

	@ViewChild(MatPaginator)
	paginator!: MatPaginator;
	@ViewChild(MatSort)
	sort!: MatSort;

	ngAfterViewInit(): void {
		this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

		merge(this.sort.sortChange, this.paginator.page)
			.pipe(
				startWith({}),
				switchMap(() => {
					this.isLoaded = false;
					return this.playersService.getPlayers();
				}),
				map((data) => {
					this.isLoaded = true;
					return data;
				})
			)
			.subscribe((data) => {
				this.playersResult = data;
			});
	}
}
