import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatToolbarModule } from '@angular/material/toolbar';
import { HomeComponent } from './components/pages/home/home.component';
import { MatButtonModule } from '@angular/material/button';
import { StatsComponent } from './components/pages/stats/stats.component';
import { PlayersComponent } from './components/pages/players/players.component';
import { PlayerTableComponent } from './components/organisms/player-table/player-table.component';
import { HttpClientModule } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { ScoutStatsComponent } from './components/pages/stats/scout-stats/scout-stats.component';
import { OverallStatsComponent } from './components/pages/stats/overall-stats/overall-stats.component';
import { SoldierStatsComponent } from './components/pages/stats/soldier-stats/soldier-stats.component';
import { DemomanStatsComponent } from './components/pages/stats/demoman-stats/demoman-stats.component';
import { MedicStatsComponent } from './components/pages/stats/medic-stats/medic-stats.component';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { SharedComponentsModule } from './components/shared-components.module';
@NgModule({
	declarations: [
		AppComponent,
		HomeComponent,
		StatsComponent,
		PlayersComponent,
		PlayerTableComponent,
		ScoutStatsComponent,
		OverallStatsComponent,
		SoldierStatsComponent,
		DemomanStatsComponent,
		MedicStatsComponent,
	],
	imports: [
		BrowserModule,
		AppRoutingModule,
		BrowserAnimationsModule,
		MatToolbarModule,
		MatButtonModule,
		MatCardModule,
		HttpClientModule,
		MatIconModule,
		MatProgressBarModule,
		MatTableModule,
		MatSidenavModule,
		MatSortModule,
		MatPaginatorModule,
		MatInputModule,
		FormsModule,
		MatButtonToggleModule,
		MatCheckboxModule,
		SharedComponentsModule,
	],
	providers: [],
	bootstrap: [AppComponent],
})
export class AppModule {}
