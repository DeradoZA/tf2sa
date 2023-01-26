import { Player } from 'src/app/models/player';

export interface GetPlayersResult {
	totalResults: number;
	count: number;
	offset: number;
	sort: string;
	sortorder: string;
	filterString: string;
	players: Player[];
}
