export interface PlayersFetchResult<TPlayer> {
	totalResults: number;
	count: number;
	offset: number;
	sort: string;
	sortOrder: string;
	filterField: string;
	filterValue: string;
	players: TPlayer[];
}
