import { PlayerProgressionPoint } from './getPlayerDetailResult';

export interface PlayerComparison {
	// String, not number: a 64-bit SteamID exceeds Number.MAX_SAFE_INTEGER.
	steamId: string;
	playerName: string | null;
	avatar: string | null;
	numberOfGames: number;
	progression: PlayerProgressionPoint[];
}

export interface GetPlayersComparisonResult {
	bucket: string;
	/** 0 = overall (cross-class); otherwise 1/2/4/7. */
	classId: number;
	/** "recent" or "alltime" — echoes back which window was actually applied. */
	period: string;
	players: PlayerComparison[];
}
