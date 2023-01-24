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
import { PlayerComponent } from './components/pages/player/player.component';
import { PlayerTableComponent } from './components/organisms/player-table/player-table.component';
import { HttpClientModule } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ErrorBannerComponent } from './components/molecules/error-banner/error-banner.component';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatInputModule } from '@angular/material/input';

@NgModule({
	declarations: [
		AppComponent,
		HomeComponent,
		StatsComponent,
		PlayersComponent,
		PlayerComponent,
		PlayerTableComponent,
		ErrorBannerComponent,
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
		MatProgressSpinnerModule,
		MatTableModule,
		MatSidenavModule,
		MatSortModule,
		MatPaginatorModule,
		MatInputModule,
	],
	providers: [],
	bootstrap: [AppComponent],
})
export class AppModule {}
