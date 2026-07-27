import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { ErrorBannerComponent } from './molecules/error-banner/error-banner.component';
import { StatsHeaderComponent } from './molecules/stats-header/stats-header.component';
import { LazyProgressionChartComponent } from './organisms/lazy-progression-chart/lazy-progression-chart.component';

/**
 * Molecules/organisms used by both eagerly-loaded pages and lazy-loaded pages.
 * A component can only be declared once, so they live here and are re-exported.
 * LazyProgressionChartComponent belongs here specifically because it has no static
 * ngx-charts import (only a dynamic one inside a method) — see that file's own doc
 * comment. Anything that DOES statically import ngx-charts (ProgressionChartComponent
 * itself) must never be added to this module — that would put d3 back in the initial
 * bundle.
 */
@NgModule({
	declarations: [
		ErrorBannerComponent,
		StatsHeaderComponent,
		LazyProgressionChartComponent,
	],
	imports: [CommonModule, MatCardModule],
	exports: [
		ErrorBannerComponent,
		StatsHeaderComponent,
		LazyProgressionChartComponent,
	],
})
export class SharedComponentsModule {}
