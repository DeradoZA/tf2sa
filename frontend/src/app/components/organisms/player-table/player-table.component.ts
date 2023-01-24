import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { MatInput } from '@angular/material/input';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { finalize, map, merge, startWith, switchMap } from 'rxjs';
import { GetPlayersResult } from 'src/app/services/players/getPlayersResult';
import {
	DEFAULT_PAGE_SIZE,
	PlayersService,
} from 'src/app/services/players/players.service';

@Component({
	selector: 'app-player-table',
	templateUrl: './player-table.component.html',
	styleUrls: ['./player-table.component.scss'],
})
export class PlayerTableComponent implements AfterViewInit {
	readonly displayedColumns: string[] = ['playerName', 'steamId'];
	constructor(private playersService: PlayersService) {}
	isLoaded: boolean = false;
	playersResult: GetPlayersResult = {
		totalResults: 0,
		count: DEFAULT_PAGE_SIZE,
		offset: 0,
		sort: '',
		sortorder: '',
		filterString: '',
		players: [],
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
					console.log(this.sort.active);
					console.log(this.sort.direction);
					return this.playersService.getPlayers(
						this.paginator.pageSize,
						this.paginator.pageIndex * this.paginator.pageSize,
						this.sort.active,
						this.sort.direction,
						''
					);
				}),
				map((data) => {
					return data;
				})
			)
			.subscribe({
				next: (result) => {
					this.playersResult = result;
					this.isLoaded = true;
				},
				error: (error) => {
					this.errorMessage = error;
					this.isLoaded = true;
				},
			});
	}
}
