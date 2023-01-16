import { HomeComponent } from './components/pages/home/home.component';
import { PlayerComponent } from './components/pages/player/player.component';
import { PlayersComponent } from './components/pages/players/players.component';
import { StatsComponent } from './components/pages/stats/stats.component';

export const ROUTES = {
	HOME: HomeComponent.PATH,
	STATS: StatsComponent.PATH,
	PLAYERS: PlayersComponent.PATH,
	PLAYER: PlayerComponent.PATH,
};
