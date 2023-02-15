import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './components/pages/home/home.component';
import { PlayerComponent } from './components/pages/player/player.component';
import { PlayersComponent } from './components/pages/players/players.component';
import { DemomanStatsComponent } from './components/pages/stats/demoman-stats/demoman-stats.component';
import { MedicStatsComponent } from './components/pages/stats/medic-stats/medic-stats.component';
import { OverallStatsComponent } from './components/pages/stats/overall-stats/overall-stats.component';
import { ScoutStatsComponent } from './components/pages/stats/scout-stats/scout-stats.component';
import { SoldierStatsComponent } from './components/pages/stats/soldier-stats/soldier-stats.component';
import { StatsComponent } from './components/pages/stats/stats.component';
import { ROUTES } from './routes';

const routes: Routes = [
	{ path: ROUTES.HOME, component: HomeComponent },
	{
		path: ROUTES.STATS,
		component: StatsComponent,
		children: [
			{ path: '', redirectTo: ROUTES.STATS_OVERALL, pathMatch: 'full' },
			{ path: ROUTES.STATS_OVERALL, component: OverallStatsComponent },
			{ path: ROUTES.STATS_SCOUT, component: ScoutStatsComponent },
			{ path: ROUTES.STATS_SOLDIER, component: SoldierStatsComponent },
			{ path: ROUTES.STATS_DEMOMAN, component: DemomanStatsComponent },
			{ path: ROUTES.STATS_MEDIC, component: MedicStatsComponent },
		],
	},
	{ path: ROUTES.PLAYERS, component: PlayersComponent },
	{ path: ROUTES.PLAYER, component: PlayerComponent },
	{ path: '', redirectTo: ROUTES.HOME, pathMatch: 'full' },
	{ path: '**', redirectTo: ROUTES.HOME },
];

@NgModule({
	imports: [RouterModule.forRoot(routes)],
	exports: [RouterModule],
})
export class AppRoutingModule {}
