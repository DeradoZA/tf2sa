import { PlayersFetchResult } from '../models/playersFetchResult';

export interface GetScoutStatsResult extends PlayersFetchResult<ScoutStat> {}

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
