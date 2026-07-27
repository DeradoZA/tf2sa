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
import {
	DEFAULT_PAGE_SIZE,
	PAGE_SIZE_OPTIONS,
} from 'src/app/services/httpConstants';
import { GetScoutStatsResult } from 'src/app/services/stats/getScoutStats';
import { StatsService } from 'src/app/services/stats/stats.service';
import { PlayersService } from 'src/app/services/players/players.service';
import { ProgressionSeriesInput } from 'src/app/components/organisms/progression-chart/progression-chart.component';

// Scout — matches the /Compare endpoint's classId convention (0=overall, 1/2/4/7).
const CLASS_ID = 1;

@Component({
	selector: 'app-scout-stats',
	templateUrl: './scout-stats.component.html',
	styleUrls: ['../stats.rank.scss'],
})
export class ScoutStatsComponent {
	readonly displayedColumns: string[] = [
		'select',
		'profilePicture',
		'playerName',
		'numberOfGames',
		'wins',
		'winPercentage',
		'draws',
		'losses',
		'averageKills',
		'averageAssists',
		'averageDeaths',
		'averageDpm',
		'averageDamageTakenPm',
		'averageHealsReceivedPm',
		'averageMedKitsHp',
		'averageCapturePointsCaptured',
		'topKills',
		'topDamage',
		'steamProfile',
	];
	readonly pageSize = DEFAULT_PAGE_SIZE;
	readonly pageSizeOptions = PAGE_SIZE_OPTIONS;

	constructor(
		private statsService: StatsService,
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

	// Graph view. hiddenPlayerIds is local (not a cross-page service, see project
	// history) and resets on every fresh table fetch — see onTableResult().
	viewMode: 'table' | 'graph' = 'table';
	hiddenPlayerIds = new Set<string>();
	// allSeries is positional against the full table page (slot = row index) and is
	// filtered, never rebuilt, to produce visibleSeries — this is what keeps a
	// player's color fixed when another row is hidden. Both are plain fields,
	// recomputed explicitly, never template getters (see progression-chart's own
	// header comment for why that discipline matters here).
	allSeries: ProgressionSeriesInput[] = [];
	visibleSeries: ProgressionSeriesInput[] = [];
	graphLoaded = false;
	graphErrorMessage: string | undefined;

	@ViewChild(MatPaginator)
	paginator!: MatPaginator;
	@ViewChild(MatSort)
	sort!: MatSort;
	public filterString: string | undefined;
	filterStringUpdate = new Subject<string>();
	statPeriodChange = new Subject();
	statPeriod = 'recent';
	refreshPress = new Subject();

	ngAfterViewInit(): void {
		merge(this.sort.sortChange, this.filterStringUpdate).subscribe(
			() => (this.paginator.pageIndex = 0)
		);

		merge(
			this.sort.sortChange,
			this.paginator.page,
			this.statPeriodChange.pipe(debounceTime(50)),
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
					return this.statsService.getScoutStats(
						this.paginator.pageSize,
						this.paginator.pageIndex * this.paginator.pageSize,
						this.sort.active,
						this.sort.direction,
						'playerName',
						this.filterString ?? '',
						this.statPeriod
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
					this.onTableResult();
				},
				error: (error) => {
					this.errorMessage = error;
					this.isLoaded = true;
				},
			});
	}

	onViewModeChange(mode: 'table' | 'graph'): void {
		this.viewMode = mode;
		if (mode === 'graph' && this.allSeries.length === 0) {
			this.fetchGraph();
		}
	}

	toggleHidden(steamId: string): void {
		if (this.hiddenPlayerIds.has(steamId)) {
			this.hiddenPlayerIds.delete(steamId);
		} else {
			this.hiddenPlayerIds.add(steamId);
		}
		this.updateVisibleSeries();
	}

	/** A fresh table page invalidates any prior graph data and hidden-set. */
	private onTableResult(): void {
		this.hiddenPlayerIds.clear();
		this.allSeries = [];
		this.visibleSeries = [];
		if (this.viewMode === 'graph') {
			this.fetchGraph();
		}
	}

	private fetchGraph(): void {
		const steamIds = this.playersResult.players.map((p) => p.steamId);
		if (steamIds.length === 0) {
			this.allSeries = [];
			this.updateVisibleSeries();
			this.graphLoaded = true;
			return;
		}

		this.graphLoaded = false;
		const period = this.statPeriod === 'recent' ? 'recent' : 'alltime';
		this.playersService
			.getPlayersComparison(steamIds, 'month', CLASS_ID, period)
			.subscribe({
				next: (result) => {
					const byId = new Map(
						result.players.map((p) => [p.steamId, p])
					);
					this.allSeries = this.playersResult.players
						.map((p, i): ProgressionSeriesInput | null => {
							const match = byId.get(p.steamId);
							if (!match) {
								return null;
							}
							return {
								id: p.steamId,
								label: p.playerName ?? p.steamId,
								slot: i,
								points: match.progression,
							};
						})
						.filter(
							(s): s is ProgressionSeriesInput => s !== null
						);
					this.updateVisibleSeries();
					this.graphErrorMessage = undefined;
					this.graphLoaded = true;
				},
				error: (error) => {
					this.graphErrorMessage = error;
					this.graphLoaded = true;
				},
			});
	}

	private updateVisibleSeries(): void {
		this.visibleSeries = this.allSeries.filter(
			(s) => !this.hiddenPlayerIds.has(s.id)
		);
	}
}
