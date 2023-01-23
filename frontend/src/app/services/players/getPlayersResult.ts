import { Player } from 'src/app/models/player';

export interface GetPlayersResult {
	totalResults: number;
	count: number;
	offset: number;
	players: Player[];
	sortBy: string;
	filterString: string;
}
