export interface GetScoutStatsResult {
	totalResults: number;
	count: number;
	offset: number;
	sort: string;
	sortOrder: string;
	players: ScoutStat[];
}

export interface ScoutStat {
	steamId: string;
	playerName: string;
	avatar: string;
	numberOfGames: number;
	averageDpm: number;
	averageKills: number;
	averageAssists: number;
	averageDeaths: number;
	topKills: number;
	topKillsGameId: number;
	topDamage: number;
	topDamageGameId: number;
}
