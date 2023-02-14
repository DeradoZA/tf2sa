import { HomeComponent } from './components/pages/home/home.component';
import { PlayerComponent } from './components/pages/player/player.component';
import { PlayersComponent } from './components/pages/players/players.component';
import { ScoutStatsComponent } from './components/pages/stats/scout-stats/scout-stats.component';
import { StatsComponent } from './components/pages/stats/stats.component';

export const ROUTES = {
	HOME: 'home',
	STATS: 'stats',
	STATS_SCOUT: 'scout',
	PLAYERS: 'players',
	PLAYER: 'players/:playerId',
};
