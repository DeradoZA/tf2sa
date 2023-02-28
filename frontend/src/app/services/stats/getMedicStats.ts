import { PlayersFetchResult } from '../models/playersFetchResult';

export interface GetMedicStatsResult extends PlayersFetchResult<MedicStat> {}

export interface MedicStat {
	steamId: string;
	playerName: string;
	avatar: string;
	numberOfGames: number;
	wins: number;
	winPercentage: number;
	draws: number;
	losses: number;
	averageKills: number;
	averageAssists: number;
	averageDeaths: number;
	averageUbers: number;
	averageDrops: number;
	averageHealsPm: number;
	topHeals: number;
	topHealsGameId: number;
	topUbers: number;
	topUbersGameId: number;
	topDrops: number;
	topDropsGameId: number;
}
