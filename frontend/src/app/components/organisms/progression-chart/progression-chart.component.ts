import {
	ChangeDetectionStrategy,
	Component,
	EventEmitter,
	Input,
	OnChanges,
	Output,
} from '@angular/core';
import { Color, LegendPosition, ScaleType } from '@swimlane/ngx-charts';
import {
	ClassOption,
	PlayerProgressionPoint,
	ProgressionBucket,
} from 'src/app/services/players/getPlayerDetailResult';
import { COMPARISON_PALETTE } from 'src/app/services/comparison/comparison-palette';

export type StatKey = keyof Pick<
	PlayerProgressionPoint,
	| 'averageDpm'
	| 'averageKills'
	| 'averageAssists'
	| 'averageDeaths'
	| 'averageAirshots'
	| 'winPercentage'
	| 'numberOfGames'
>;

/** A flat lookup of one player's career averages, keyed the same as a stat option. */
export type CareerValues = Partial<Record<StatKey, number>>;

/**
 * One line on the chart. `id` is the stable identity (a SteamID) and `slot` is a fixed
 * palette index assigned by the caller — color must follow the player, never the
 * player's position in this array, so removing one player must never repaint another's
 * line. Single-player callers (the player page) pass one entry; a leaderboard's Graph
 * view passes everyone currently on the table page (up to the largest page size, 100) —
 * slots repeat past the 8-hue palette at that point, see COMPARISON_PALETTE below.
 */
export interface ProgressionSeriesInput {
	id: string;
	label: string;
	slot: number;
	points: PlayerProgressionPoint[];
}

interface StatOption {
	key: StatKey;
	label: string;
	/** The matching field on CareerValues, if this stat has a career figure. */
	careerField: StatKey | null;
}

interface ChartSeries {
	name: string;
	series: { name: Date; value: number }[];
}

interface ReferenceLine {
	name: string;
	value: number;
}

interface CustomColor {
	name: string;
	value: string;
}

// COMPARISON_PALETTE (imported above) is used for the multi-player case — fixed slot
// order, validated at the chart's own plot background (#262626, darker than the app's
// #424242 card so every slot clears the 3:1 contrast obligation). Above 8 series,
// colors intentionally repeat (`slot % PALETTE_LENGTH`, in recompute() below) — a
// leaderboard's Graph view can show up to 100 players at once, and no palette makes
// that many hues individually distinguishable. The hover tooltip (ngx-charts default,
// never disabled here) is the disambiguator once colors repeat; see the legend-count
// threshold below for the companion adjustment once the *legend* also stops being
// useful at high counts.

// The single-player case keeps the app's own brand accent (green A200, $tf2sa-text)
// rather than the comparison palette — this is what was validated visually last round
// and there's no identity-vs-rank concern with exactly one line.
const SINGLE_SERIES_COLOR = '#69f0ae';

// Past this many series, an ngx-charts legend stops being a usable legend (it's just a
// long scrolling list of names) — hide it and rely on the hover tooltip alone, the same
// disambiguator already carrying the load once colors start repeating past 8.
const MAX_LEGEND_ENTRIES = 20;

/**
 * NOTE: nothing bound in this template may be a getter that builds a new object.
 * ngx-charts re-runs its scale and render work whenever `results` changes identity, so a
 * getter returning a fresh array is re-rendered on every change-detection pass — which
 * with a few hundred points (and up to 100 series here) pegs the main thread and never
 * settles. Everything the template reads is a field, recomputed only in `recompute()`.
 */
@Component({
	selector: 'app-progression-chart',
	templateUrl: './progression-chart.component.html',
	styleUrls: ['./progression-chart.component.scss'],
	changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProgressionChartComponent implements OnChanges {
	@Input() series: ProgressionSeriesInput[] = [];
	/** Career averages for the reference line — meaningful only for a single series. */
	@Input() careerValues: CareerValues | null = null;
	@Input() bucket: ProgressionBucket = 'month';
	@Output() bucketChange = new EventEmitter<ProgressionBucket>();

	/** Optional single-player class filter — only the player page sets these; a
	 * leaderboard's Graph view leaves them undefined, so the toolbar renders no
	 * extra control. */
	@Input() classOptions?: ClassOption[];
	@Input() classId?: number;
	@Output() classIdChange = new EventEmitter<number>();

	onClassSelect(classId: number): void {
		this.classIdChange.emit(classId);
	}

	readonly statOptions: StatOption[] = [
		{
			key: 'averageDpm',
			label: 'Damage per minute',
			careerField: 'averageDpm',
		},
		{
			key: 'averageKills',
			label: 'Kills per game',
			careerField: 'averageKills',
		},
		{
			key: 'averageAssists',
			label: 'Assists per game',
			careerField: 'averageAssists',
		},
		{
			key: 'averageDeaths',
			label: 'Deaths per game',
			careerField: 'averageDeaths',
		},
		{
			key: 'averageAirshots',
			label: 'Airshots per game',
			careerField: 'averageAirshots',
		},
		{ key: 'winPercentage', label: 'Win %', careerField: 'winPercentage' },
		// Games per bucket has no career-average equivalent — the reference line falls
		// back to the mean across buckets, which is the right meaning here.
		{ key: 'numberOfGames', label: 'Games played', careerField: null },
	];
	selectedStat: StatKey = 'averageDpm';

	readonly colorScheme: Color = {
		name: 'progression',
		selectable: false,
		group: ScaleType.Ordinal,
		domain: [SINGLE_SERIES_COLOR, ...COMPARISON_PALETTE],
	};

	// Right, not Below: ngx-charts only reserves space for the legend (splitting
	// chart/legend width) when position is Right — Below leaves the SVG at full
	// height with the legend rendered as an overlapping sibling, not stacked beneath
	// it. The legend's own list has overflow-y: auto built in, so a tall entry count
	// scrolls inside its column rather than overflowing.
	readonly legendPosition = LegendPosition.Right;

	chartData: ChartSeries[] = [];
	customColors: CustomColor[] = [];
	referenceLines: ReferenceLine[] = [];
	hasData = false;
	showLegend = false;
	selectedLabel = '';
	xAxisLabel = '';

	ngOnChanges(): void {
		this.recompute();
	}

	/**
	 * For callers that create this component imperatively (LazyProgressionChartComponent,
	 * via ViewContainerRef.createComponent) rather than through a template binding:
	 * Angular only invokes ngOnChanges for template-bound `[x]="y"` inputs, so setting
	 * `.instance.series = ...` directly does not trigger it. Call this instead.
	 */
	refresh(): void {
		this.recompute();
	}

	onStatChange(stat: StatKey): void {
		this.selectedStat = stat;
		this.recompute();
	}

	onBucketToggle(bucket: ProgressionBucket): void {
		if (bucket !== this.bucket) {
			this.bucketChange.emit(bucket);
		}
	}

	private recompute(): void {
		const option = this.statOptions.find((o) => o.key === this.selectedStat);
		this.selectedLabel = option?.label ?? '';
		this.xAxisLabel = this.bucket === 'week' ? 'Week' : 'Month';

		this.chartData = this.series.map((s) => ({
			name: s.label,
			series: s.points
				.filter((p) => p[this.selectedStat] !== null)
				.map((p) => ({
					name: new Date(p.bucket),
					value: p[this.selectedStat] as number,
				})),
		}));
		this.hasData = this.chartData.some((s) => s.series.length > 0);
		this.showLegend =
			this.series.length > 1 && this.series.length <= MAX_LEGEND_ENTRIES;

		// customColors keys by series NAME (a string match, resolved every render), not
		// array position — this is what actually guarantees a removed player's line
		// never repaints a survivor's, since ngx-charts' default `scheme.domain` colors
		// by position in `results` and would repaint everyone after a removal.
		this.customColors =
			this.series.length === 1
				? [{ name: this.series[0].label, value: SINGLE_SERIES_COLOR }]
				: this.series.map((s) => ({
						name: s.label,
						value: COMPARISON_PALETTE[s.slot % COMPARISON_PALETTE.length],
				  }));

		this.referenceLines = this.buildReferenceLine(option);
	}

	/**
	 * The career average, taken from the caller's summary so it matches whatever
	 * headline figure is shown elsewhere on the page exactly. It cannot be re-derived
	 * from the buckets: weighting them by NumberOfGames misses that a bucket counts
	 * medic-only games whose damage stats are absent from its average, and each bucket
	 * is already rounded. Only shown for exactly one series — a dashed reference line
	 * per player would be noise at any multi-player count, and multi-player mode never
	 * supplies careerValues.
	 */
	private buildReferenceLine(option: StatOption | undefined): ReferenceLine[] {
		if (!option || this.series.length !== 1) {
			return [];
		}
		const points = this.series[0].points.filter(
			(p) => p[this.selectedStat] !== null
		);
		if (points.length === 0) {
			return [];
		}

		let value: number | null = null;
		if (option.careerField && this.careerValues) {
			const careerValue = this.careerValues[option.careerField];
			value = typeof careerValue === 'number' ? careerValue : null;
		} else if (!option.careerField) {
			const total = points.reduce(
				(sum, p) => sum + (p[this.selectedStat] as number),
				0
			);
			value = Math.round((total / points.length) * 10) / 10;
		}

		return value === null ? [] : [{ name: 'Career avg', value: value }];
	}
}
