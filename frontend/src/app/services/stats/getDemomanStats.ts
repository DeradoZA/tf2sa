import { PlayersFetchResult } from '../models/playersFetchResult';

export interface GetDemomanStatsResult
	extends PlayersFetchResult<DemomanStat> {}

export interface DemomanStat {
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
	averageAirshots: number;
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
