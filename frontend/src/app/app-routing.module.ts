import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './components/pages/home/home.component';
import { PlayerComponent } from './components/pages/player/player.component';
import { PlayersComponent } from './components/pages/players/players.component';
import { StatsComponent } from './components/pages/stats/stats.component';
import { ROUTES } from './routes';

const routes: Routes = [
	{ path: ROUTES.HOME, component: HomeComponent },
	{ path: ROUTES.STATS, component: StatsComponent },
	{ path: ROUTES.PLAYERS, component: PlayersComponent },
	{ path: ROUTES.PLAYER, component: PlayerComponent },
	{ path: '', redirectTo: HomeComponent.PATH, pathMatch: 'full' },
	{ path: '**', redirectTo: HomeComponent.PATH },
];

@NgModule({
	imports: [RouterModule.forRoot(routes)],
	exports: [RouterModule],
})
export class AppRoutingModule {}
