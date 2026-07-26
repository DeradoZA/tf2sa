import {
	ChangeDetectionStrategy,
	Component,
	ComponentRef,
	EventEmitter,
	Input,
	OnChanges,
	Output,
	ViewChild,
	ViewContainerRef,
} from '@angular/core';
// Type-only: erased at compile time, so this creates no module dependency and cannot
// pull ngx-charts into whichever bundle this file lands in.
import type {
	CareerValues,
	ProgressionChartComponent,
	ProgressionSeriesInput,
} from '../progression-chart/progression-chart.component';
import { ProgressionBucket } from 'src/app/services/players/getPlayerDetailResult';

/**
 * Loads `ProgressionChartComponent` — and everything it drags in (`NgxChartsModule`,
 * d3) — only when actually used, via a dynamic `import()` resolved the first time
 * inputs arrive. This file itself has no static ngx-charts import, so it's safe to
 * declare eagerly (in SharedComponentsModule) and drop onto the 5 always-loaded
 * leaderboard pages: the eager bundle stays exactly as it was before this component
 * existed, and the real chart's code downloads only on first use, in the same shared
 * lazy chunk the player-detail and (previously) compare pages already used.
 *
 * A component created via `ViewContainerRef.createComponent()` has its inputs set
 * imperatively, which does NOT trigger Angular's `ngOnChanges` (that hook only fires
 * for template-bound `[x]="y"` inputs) — so every input push here is followed by an
 * explicit call to the real chart's public `refresh()` method instead of relying on
 * the lifecycle hook.
 */
@Component({
	selector: 'app-lazy-progression-chart',
	template: `
		<ng-container #host></ng-container>
		<p *ngIf="loading" class="loading-hint">Loading chart…</p>
	`,
	styles: [
		`
			.loading-hint {
				opacity: 0.7;
				font-style: italic;
			}
		`,
	],
	changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LazyProgressionChartComponent implements OnChanges {
	@ViewChild('host', { read: ViewContainerRef, static: true })
	private host!: ViewContainerRef;

	@Input() series: ProgressionSeriesInput[] = [];
	@Input() careerValues: CareerValues | null = null;
	@Input() bucket: ProgressionBucket = 'month';
	@Output() bucketChange = new EventEmitter<ProgressionBucket>();

	loading = false;

	private componentRef?: ComponentRef<ProgressionChartComponent>;
	private loadPromise?: Promise<void>;

	ngOnChanges(): void {
		if (this.componentRef) {
			this.applyInputs();
			return;
		}
		this.loadPromise ??= this.load();
	}

	private async load(): Promise<void> {
		this.loading = true;
		const { ProgressionChartComponent: RealChart } = await import(
			'../progression-chart/progression-chart.component'
		);
		this.host.clear();
		this.componentRef = this.host.createComponent(RealChart);
		this.componentRef.instance.bucketChange.subscribe(
			(bucket: ProgressionBucket) => this.bucketChange.emit(bucket)
		);
		this.loading = false;
		this.applyInputs();
	}

	private applyInputs(): void {
		if (!this.componentRef) {
			return;
		}
		this.componentRef.instance.series = this.series;
		this.componentRef.instance.careerValues = this.careerValues;
		this.componentRef.instance.bucket = this.bucket;
		this.componentRef.instance.refresh();
		this.componentRef.changeDetectorRef.detectChanges();
	}
}
