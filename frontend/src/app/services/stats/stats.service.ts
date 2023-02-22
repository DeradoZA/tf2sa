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
import { GetSoldierStatsResult } from './getSoldierStats';
import { GetDemomanStatsResult } from './getDemomanStats';
import { GetMedicStatsResult } from './getMedicStats';

const STATS_BASE_URL = `${environment.backendUrl}/Statistics`;

@Injectable({
	providedIn: 'root',
})
export class StatsService {
	constructor(private httpClient: HttpClient) {}

	getScoutStats(
		count = DEFAULT_PAGE_SIZE,
		offset = 0,
		sort = 'averageDpm',
		sortorder = 'desc',
		filterField = 'playerName',
		filterValue = '',
		period = 'recent'
	) {
		let params = new HttpParams();
		params = params.append('count', count.toString());
		params = params.append('offset', offset.toString());
		params = params.append('sort', sort);
		params = params.append('sortorder', sortorder);
		params = params.append('filterField', filterField);
		params = params.append('filterValue', filterValue);

		if (period === 'alltime') {
			return this.httpClient
				.get<GetScoutStatsResult>(`${STATS_BASE_URL}/ScoutAllTime`, {
					params: params,
				})
				.pipe(catchError(this.handleError));
		}
		return this.httpClient
			.get<GetScoutStatsResult>(`${STATS_BASE_URL}/ScoutRecent`, {
				params: params,
			})
			.pipe(catchError(this.handleError));
	}

	getSoldierStats(
		count = DEFAULT_PAGE_SIZE,
		offset = 0,
		sort = 'averageDpm',
		sortorder = 'desc',
		filterField = 'playerName',
		filterValue = '',
		period = 'recent'
	) {
		let params = new HttpParams();
		params = params.append('count', count.toString());
		params = params.append('offset', offset.toString());
		params = params.append('sort', sort);
		params = params.append('sortorder', sortorder);
		params = params.append('filterField', filterField);
		params = params.append('filterValue', filterValue);

		if (period === 'alltime') {
			return this.httpClient
				.get<GetSoldierStatsResult>(
					`${STATS_BASE_URL}/SoldierAllTime`,
					{
						params: params,
					}
				)
				.pipe(catchError(this.handleError));
		}
		return this.httpClient
			.get<GetSoldierStatsResult>(`${STATS_BASE_URL}/SoldierRecent`, {
				params: params,
			})
			.pipe(catchError(this.handleError));
	}

	getDemomanStats(
		count = DEFAULT_PAGE_SIZE,
		offset = 0,
		sort = 'averageDpm',
		sortorder = 'desc',
		filterField = 'playerName',
		filterValue = '',
		period = 'recent'
	) {
		let params = new HttpParams();
		params = params.append('count', count.toString());
		params = params.append('offset', offset.toString());
		params = params.append('sort', sort);
		params = params.append('sortorder', sortorder);
		params = params.append('filterField', filterField);
		params = params.append('filterValue', filterValue);

		if (period === 'alltime') {
			return this.httpClient
				.get<GetDemomanStatsResult>(
					`${STATS_BASE_URL}/DemomanAllTime`,
					{
						params: params,
					}
				)
				.pipe(catchError(this.handleError));
		}
		return this.httpClient
			.get<GetDemomanStatsResult>(`${STATS_BASE_URL}/DemomanRecent`, {
				params: params,
			})
			.pipe(catchError(this.handleError));
	}

	getMedicStats(
		count = DEFAULT_PAGE_SIZE,
		offset = 0,
		sort = 'averageDpm',
		sortorder = 'desc',
		filterField = 'playerName',
		filterValue = '',
		period = 'recent'
	) {
		let params = new HttpParams();
		params = params.append('count', count.toString());
		params = params.append('offset', offset.toString());
		params = params.append('sort', sort);
		params = params.append('sortorder', sortorder);
		params = params.append('filterField', filterField);
		params = params.append('filterValue', filterValue);

		if (period === 'alltime') {
			return this.httpClient
				.get<GetMedicStatsResult>(`${STATS_BASE_URL}/MedicAllTime`, {
					params: params,
				})
				.pipe(catchError(this.handleError));
		}
		return this.httpClient
			.get<GetMedicStatsResult>(`${STATS_BASE_URL}/MedicRecent`, {
				params: params,
			})
			.pipe(catchError(this.handleError));
	}

	private handleError(error: HttpErrorResponse) {
		return throwError(() => new Error(error.statusText));
	}
}
