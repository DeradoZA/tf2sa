import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { ProgressionChartComponent } from './progression-chart.component';

/**
 * Owns ngx-charts (and its d3 dependencies) in one place so both lazy feature modules
 * that need a progression chart (player detail, comparison) import it the same way,
 * rather than each pulling ngx-charts in directly. Must only ever be imported by a
 * LAZY module — importing it from AppModule would put d3 back in the initial bundle
 * and re-breach the 1 MB budget that made this page lazy in the first place.
 */
@NgModule({
	declarations: [ProgressionChartComponent],
	imports: [
		CommonModule,
		MatButtonToggleModule,
		MatCardModule,
		MatSelectModule,
		NgxChartsModule,
	],
	exports: [ProgressionChartComponent],
})
export class ProgressionChartModule {}
