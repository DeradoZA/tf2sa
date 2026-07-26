import { Injectable } from '@angular/core';
import {
	HttpClient,
	HttpErrorResponse,
	HttpParams,
} from '@angular/common/http';
import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { GetPlayersResult } from './getPlayersResult';
import {
	GetPlayerDetailResult,
	ProgressionBucket,
} from './getPlayerDetailResult';
import { GetPlayersComparisonResult } from './getPlayersComparisonResult';
import { environment } from 'src/environments/environment';
import { DEFAULT_PAGE_SIZE } from '../httpConstants';

const PLAYERS_BASE_URL = `${environment.backendUrl}/Players`;

@Injectable({
	providedIn: 'root',
})
export class PlayersService {
	constructor(private httpClient: HttpClient) {}

	getPlayers(
		count = DEFAULT_PAGE_SIZE,
		offset = 0,
		sort = 'playerName',
		sortorder = 'asc',
		filterString: string
	) {
		let params = new HttpParams();
		params = params.append('count', count.toString());
		params = params.append('offset', offset.toString());
		params = params.append('sort', sort);
		params = params.append('sortorder', sortorder);
		params = params.append('filterString', filterString);

		return this.httpClient
			.get<GetPlayersResult>(PLAYERS_BASE_URL, { params: params })
			.pipe(catchError(this.handleError));
	}

	getPlayerDetail(
		steamId: string,
		bucket: ProgressionBucket = 'month',
		classId = 0
	) {
		let params = new HttpParams();
		params = params.append('bucket', bucket);
		params = params.append('classId', classId.toString());

		return this.httpClient
			.get<GetPlayerDetailResult>(
				`${PLAYERS_BASE_URL}/${steamId}/Detail`,
				{
					params: params,
				}
			)
			.pipe(catchError(this.handleError));
	}

	/** steamIds must be non-empty and at most 100 — the backend rejects otherwise (400). */
	getPlayersComparison(
		steamIds: string[],
		bucket: ProgressionBucket = 'month',
		classId = 0,
		period: 'recent' | 'alltime' = 'alltime'
	) {
		let params = new HttpParams();
		params = params.append('steamIds', steamIds.join(','));
		params = params.append('bucket', bucket);
		params = params.append('classId', classId.toString());
		params = params.append('period', period);

		return this.httpClient
			.get<GetPlayersComparisonResult>(`${PLAYERS_BASE_URL}/Compare`, {
				params: params,
			})
			.pipe(catchError(this.handleError));
	}

	private handleError(error: HttpErrorResponse) {
		return throwError(() => new Error(error.statusText));
	}
}
