import { PlayersFetchResult } from '../models/playersFetchResult';

export interface GetScoutStatsResult extends PlayersFetchResult<ScoutStat> {}

export interface ScoutStat {
	steamId: string;
	playerName: string;
	avatar: string;
	numberOfGames: number;
	wins: number;
	winPercentage: number;
	draws: number;
	losses: number;
	averageDpm: number;
	averageKills: number;
	averageAssists: number;
	averageDeaths: number;
	averageDamageTakenPm: number;
	averageHealsReceivedPm: number;
	averageMedKitsHp: number;
	averageCapturePointsCaptured: number;
	topKills: number;
	topKillsGameId: number;
	topDamage: number;
	topDamageGameId: number;
}
