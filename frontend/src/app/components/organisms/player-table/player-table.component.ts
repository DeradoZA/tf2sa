import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { MatIconRegistry } from '@angular/material/icon';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { DomSanitizer } from '@angular/platform-browser';
import {
	debounceTime,
	distinctUntilChanged,
	map,
	merge,
	startWith,
	Subject,
	switchMap,
} from 'rxjs';
import { DEFAULT_PAGE_SIZE } from 'src/app/services/httpConstants';
import { GetPlayersResult } from 'src/app/services/players/getPlayersResult';
import { PlayersService } from 'src/app/services/players/players.service';

@Component({
	selector: 'app-player-table',
	templateUrl: './player-table.component.html',
	styleUrls: ['./player-table.component.scss'],
})
export class PlayerTableComponent implements AfterViewInit {
	readonly displayedColumns: string[] = [
		'profilePicture',
		'playerName',
		'steamId',
		'steamProfile',
	];
	readonly pageSize = DEFAULT_PAGE_SIZE;
	constructor(
		private playersService: PlayersService,
		private iconRegistry: MatIconRegistry,
		private sanitizer: DomSanitizer
	) {
		this.iconRegistry.addSvgIcon(
			'steam-icon',
			this.sanitizer.bypassSecurityTrustResourceUrl(
				'assets/svg-icons/icons8-steam-circled.svg'
			)
		);
		this.iconRegistry.addSvgIcon(
			'refresh-icon',
			this.sanitizer.bypassSecurityTrustResourceUrl(
				'assets/svg-icons/icons8-restart.svg'
			)
		);
	}
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
	public filterString: string | undefined;
	filterStringUpdate = new Subject<string>();
	refreshPress = new Subject();

	ngAfterViewInit(): void {
		merge(this.sort.sortChange, this.filterStringUpdate).subscribe(
			() => (this.paginator.pageIndex = 0)
		);

		merge(
			this.sort.sortChange,
			this.paginator.page,
			this.filterStringUpdate.pipe(
				debounceTime(600),
				distinctUntilChanged()
			),
			this.refreshPress
		)
			.pipe(
				startWith({}),
				switchMap(() => {
					this.isLoaded = false;
					return this.playersService.getPlayers(
						this.paginator.pageSize,
						this.paginator.pageIndex * this.paginator.pageSize,
						this.sort.active,
						this.sort.direction,
						this.filterString ?? ''
					);
				}),
				map((data) => {
					return data;
				})
			)
			.subscribe({
				next: (result) => {
					this.playersResult = result;
					this.errorMessage = undefined;
					this.isLoaded = true;
				},
				error: (error) => {
					this.errorMessage = error;
					this.isLoaded = true;
				},
			});
	}
}
