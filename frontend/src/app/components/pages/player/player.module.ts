import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { PlayerComponent } from './player.component';
import { ProgressionChartModule } from '../../organisms/progression-chart/progression-chart.module';
import { PlayerClassSplitsTableComponent } from '../../organisms/player-class-splits-table/player-class-splits-table.component';
import { PlayerMapSplitsTableComponent } from '../../organisms/player-map-splits-table/player-map-splits-table.component';
import { PlayerWeaponSplitsTableComponent } from '../../organisms/player-weapon-splits-table/player-weapon-splits-table.component';
import { PlayerSynergyTableComponent } from '../../organisms/player-synergy-table/player-synergy-table.component';
import { SharedComponentsModule } from '../../shared-components.module';

const routes: Routes = [{ path: '', component: PlayerComponent }];

/**
 * Lazy-loaded so ngx-charts and its d3 dependencies (pulled in via ProgressionChartModule)
 * stay out of the initial bundle — they are only needed on this page and the comparison
 * page, and eager-loading them pushed the app past its bundle budget.
 */
@NgModule({
	declarations: [
		PlayerComponent,
		PlayerClassSplitsTableComponent,
		PlayerMapSplitsTableComponent,
		PlayerWeaponSplitsTableComponent,
		PlayerSynergyTableComponent,
	],
	imports: [
		CommonModule,
		RouterModule.forChild(routes),
		SharedComponentsModule,
		ProgressionChartModule,
		MatCardModule,
		MatProgressBarModule,
		MatSelectModule,
		MatTableModule,
	],
})
export class PlayerModule {}
