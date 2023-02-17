import { Component, ViewChild } from '@angular/core';
import { MatIconRegistry } from '@angular/material/icon';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { DomSanitizer } from '@angular/platform-browser';
import {
	Subject,
	merge,
	debounceTime,
	distinctUntilChanged,
	startWith,
	switchMap,
	map,
} from 'rxjs';
import { DEFAULT_PAGE_SIZE } from 'src/app/services/httpConstants';
import { GetScoutStatsResult } from 'src/app/services/stats/getScoutStats';
import { StatsService } from 'src/app/services/stats/stats.service';

@Component({
	selector: 'app-scout-stats',
	templateUrl: './scout-stats.component.html',
	styleUrls: ['./scout-stats.component.scss'],
})
export class ScoutStatsComponent {
	readonly displayedColumns: string[] = [
		'profilePicture',
		'playerName',
		'numberOfGames',
		'averageDpm',
		'averageKills',
		'averageAssists',
		'averageDeaths',
		'topKills',
		'topDamage',
		'steamProfile',
	];
	readonly pageSize = DEFAULT_PAGE_SIZE;
	constructor(
		private statsService: StatsService,
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
	playersResult: GetScoutStatsResult = {
		totalResults: 0,
		count: DEFAULT_PAGE_SIZE,
		offset: 0,
		sort: '',
		sortOrder: '',
		players: [],
		filterField: '',
		filterValue: '',
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
					return this.statsService.getScoutRecentStats(
						this.paginator.pageSize,
						this.paginator.pageIndex * this.paginator.pageSize,
						this.sort.active,
						this.sort.direction,
						'playerName',
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
