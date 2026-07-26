export interface PlayerSummary {
	// String, not number: a 64-bit SteamID exceeds Number.MAX_SAFE_INTEGER.
	steamId: string;
	playerName: string | null;
	avatar: string | null;
	numberOfGames: number;
	wins: number;
	draws: number;
	losses: number;
	winPercentage: number | null;
	// Career averages — the progression chart draws these as its reference line.
	averageDpm: number | null;
	averageKills: number | null;
	averageDeaths: number | null;
	averageAssists: number | null;
	averageAirshots: number | null;
}

export interface PlayerProgressionPoint {
	bucket: string;
	numberOfGames: number;
	wins: number;
	draws: number;
	losses: number;
	winPercentage: number | null;
	averageDpm: number | null;
	averageKills: number | null;
	averageDeaths: number | null;
	averageAssists: number | null;
	averageAirshots: number | null;
}

export interface PlayerClassSplit {
	classId: number;
	numberOfGames: number;
	wins: number;
	averageDpm: number | null;
	averageKills: number | null;
	averageAssists: number | null;
	averageDeaths: number | null;
}

export interface GetPlayerDetailResult {
	// Null when the player has no recorded games.
	summary: PlayerSummary | null;
	bucket: string;
	/** Which class `progression` is scoped to — 0 (overall) or 1/2/4/7. */
	classId: number;
	progression: PlayerProgressionPoint[];
	classSplits: PlayerClassSplit[];
}

export type ProgressionBucket = 'week' | 'month';

// Matches the backend's classId convention used throughout (leaderboards, compare,
// player detail): 0 = overall (cross-class), otherwise the class's own board.
export interface ClassOption {
	classId: number;
	label: string;
}

export const CLASS_OPTIONS: ClassOption[] = [
	{ classId: 0, label: 'Overall' },
	{ classId: 1, label: 'Scout' },
	{ classId: 2, label: 'Soldier' },
	{ classId: 4, label: 'Demoman' },
	{ classId: 7, label: 'Medic' },
];

// "Damage classes" — the default-selection pool this page picks from (see
// player.component.ts), excluding Medic and Overall.
export const DAMAGE_CLASS_IDS: readonly number[] = [1, 2, 4];
