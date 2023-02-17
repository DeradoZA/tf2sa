import { Injectable } from '@angular/core';
import {
	HttpClient,
	HttpErrorResponse,
	HttpParams,
} from '@angular/common/http';
import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { DEFAULT_PAGE_SIZE } from '../httpConstants';
import { GetScoutStatsResult } from './getScoutStats';

const STATS_BASE_URL = `${environment.backendUrl}/Statistics`;

@Injectable({
	providedIn: 'root',
})
export class StatsService {
	constructor(private httpClient: HttpClient) {}

	getScoutRecentStats(
		count = DEFAULT_PAGE_SIZE,
		offset = 0,
		sort = 'averageDpm',
		sortorder = 'desc',
		filterField = 'playerName',
		filterValue = ''
	) {
		let params = new HttpParams();
		params = params.append('count', count.toString());
		params = params.append('offset', offset.toString());
		params = params.append('sort', sort);
		params = params.append('sortorder', sortorder);
		params = params.append('filterField', filterField);
		params = params.append('filterValue', filterValue);

		return this.httpClient
			.get<GetScoutStatsResult>(`${STATS_BASE_URL}/ScoutRecent`, {
				params: params,
			})
			.pipe(catchError(this.handleError));
	}

	private handleError(error: HttpErrorResponse) {
		return throwError(() => new Error(error.statusText));
	}
}
