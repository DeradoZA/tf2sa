import { PlayersFetchResult } from '../models/playersFetchResult';

export interface GetOverallStatsResult
	extends PlayersFetchResult<OverallStat> {}

export interface OverallStat {
	steamId: string;
	playerName: string;
	avatar: string;
	numberOfGames: number;
	wins: number;
	draws: number;
	losses: number;
	averageDpm: number;
	averageKills: number;
	averageAssists: number;
	averageDeaths: number;
	averageAirshots: number;
	averageHeadshots: number;
	averageBackstabs: number;
	averageDamageTakenPm: number;
	averageHealsReceivedPm: number;
	averageMedKitsHp: number;
	averageCapturePointsCaptured: number;
	topKills: number;
	topKillsGameId: number;
	topDamage: number;
	topDamageGameId: number;
	topAirshots: number;
	topAirshotsGameId: number;
}
