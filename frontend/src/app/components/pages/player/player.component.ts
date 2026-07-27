import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subject, startWith, switchMap } from 'rxjs';
import {
	CLASS_OPTIONS,
	ClassOption,
	DAMAGE_CLASS_IDS,
	GetPlayerDetailResult,
	PlayerClassSplit,
	ProgressionBucket,
} from 'src/app/services/players/getPlayerDetailResult';
import { PlayersService } from 'src/app/services/players/players.service';
import {
	CareerValues,
	ProgressionSeriesInput,
} from 'src/app/components/organisms/progression-chart/progression-chart.component';

@Component({
	selector: 'app-player',
	templateUrl: './player.component.html',
	styleUrls: ['./player.component.scss'],
})
export class PlayerComponent implements OnInit {
	// String, not number: a 64-bit SteamID exceeds Number.MAX_SAFE_INTEGER, so
	// Number() would silently lose precision and request the wrong player.
	playerId: string | undefined;

	detail: GetPlayerDetailResult | undefined;
	isLoaded = false;
	errorMessage: string | undefined;

	bucket: ProgressionBucket = 'month';
	readonly classOptions: ClassOption[] = CLASS_OPTIONS;
	// 0 (Overall) until the first response's classSplits picks a real default — see
	// applyDefaultClassIfNeeded(). A user-driven change short-circuits that default.
	classId = 0;
	private defaultClassApplied = false;

	// Fetch trigger — bucket and class changes both land here; the value doesn't
	// matter, only that a new fetch should run with whatever bucket/classId currently
	// hold. Kept as one Subject rather than two so ngOnInit only needs one pipeline.
	private refresh = new Subject<void>();

	// Built once per response, not via a template getter — see the same rule in
	// progression-chart.component.ts (a getter here would rebuild these on every
	// change-detection pass and undo the reason that component is OnPush).
	chartSeries: ProgressionSeriesInput[] = [];
	careerValues: CareerValues | null = null;

	constructor(
		private route: ActivatedRoute,
		private playersService: PlayersService
	) {}

	ngOnInit(): void {
		this.playerId = this.route.snapshot.paramMap.get('playerId') ?? undefined;

		this.refresh
			.pipe(
				startWith(undefined),
				switchMap(() => {
					this.isLoaded = false;
					return this.playersService.getPlayerDetail(
						this.playerId ?? '',
						this.bucket,
						this.classId
					);
				})
			)
			.subscribe({
				next: (result) => {
					if (this.applyDefaultClassIfNeeded(result)) {
						// classId changed and a re-fetch scoped to it is already in
						// flight — rendering this (still-Overall) response would just
						// flash the wrong class for an instant, so skip it.
						return;
					}
					this.detail = result;
					this.errorMessage = undefined;
					this.isLoaded = true;
					this.updateChartInputs(result);
				},
				error: (error) => {
					this.errorMessage = error;
					this.isLoaded = true;
				},
			});
	}

	/** The chart owns the bucket control; fetching stays here. */
	onBucketChange(bucket: ProgressionBucket): void {
		this.bucket = bucket;
		this.refresh.next();
	}

	onClassChange(classId: number): void {
		this.classId = classId;
		this.refresh.next();
	}

	get notFound(): boolean {
		return this.isLoaded && !this.errorMessage && !this.detail?.summary;
	}

	/**
	 * Runs once, on the first successful response (always fetched as Overall). Picks
	 * the player's most-played "damage" class (Scout/Soldier/Demoman — Medic and
	 * Overall itself are excluded from this pool) as the default view, matching what
	 * a visitor most likely wants to see first. Returns true if it triggered a
	 * re-fetch scoped to that class, in which case the caller should not render the
	 * (still-Overall) response it was just given.
	 */
	private applyDefaultClassIfNeeded(result: GetPlayerDetailResult): boolean {
		if (this.defaultClassApplied) {
			return false;
		}
		this.defaultClassApplied = true;

		const defaultClassId = this.pickDefaultDamageClass(result.classSplits);
		if (defaultClassId === this.classId) {
			return false;
		}
		this.classId = defaultClassId;
		this.refresh.next();
		return true;
	}

	private pickDefaultDamageClass(classSplits: PlayerClassSplit[]): number {
		const damageSplits = classSplits.filter((c) =>
			DAMAGE_CLASS_IDS.includes(c.classId)
		);
		if (damageSplits.length === 0) {
			return 0;
		}
		return damageSplits.reduce((best, c) =>
			c.numberOfGames > best.numberOfGames ? c : best
		).classId;
	}

	private updateChartInputs(result: GetPlayerDetailResult): void {
		const summary = result.summary;
		if (!summary) {
			this.chartSeries = [];
			this.careerValues = null;
			return;
		}

		this.chartSeries = [
			{
				id: summary.steamId,
				label: summary.playerName ?? summary.steamId,
				slot: 0,
				points: result.progression,
			},
		];
		this.careerValues = this.buildCareerValues(result);
	}

	/**
	 * The reference line's career values must match whatever class the progression
	 * series is scoped to — Overall uses the player_summary career averages already
	 * fetched; a specific class uses that class's row from classSplits instead, so
	 * e.g. selecting Soldier shows the Soldier career DPM, not the cross-class one.
	 */
	private buildCareerValues(
		result: GetPlayerDetailResult
	): CareerValues | null {
		if (result.classId === 0) {
			const summary = result.summary;
			return summary
				? {
						averageDpm: summary.averageDpm ?? undefined,
						averageKills: summary.averageKills ?? undefined,
						averageDeaths: summary.averageDeaths ?? undefined,
						averageAssists: summary.averageAssists ?? undefined,
						averageAirshots: summary.averageAirshots ?? undefined,
						winPercentage: summary.winPercentage ?? undefined,
				  }
				: null;
		}

		const classSplit = result.classSplits.find(
			(c) => c.classId === result.classId
		);
		if (!classSplit) {
			return null;
		}
		return {
			averageDpm: classSplit.averageDpm ?? undefined,
			averageKills: classSplit.averageKills ?? undefined,
			averageDeaths: classSplit.averageDeaths ?? undefined,
			averageAssists: classSplit.averageAssists ?? undefined,
			// player_class_splits doesn't select Airshots (it's a per-game, not
			// per-class, PlayerStats field — see that macro's own comment) — the
			// reference line simply won't show for Airshots when a specific class is
			// selected. Overall still has it via player_summary above.
			averageAirshots: undefined,
			winPercentage:
				classSplit.numberOfGames > 0
					? Math.round(
							(classSplit.wins / classSplit.numberOfGames) * 1000
					  ) / 10
					: undefined,
		};
	}
}
