import { Injectable } from '@angular/core';
import {
	HttpClient,
	HttpErrorResponse,
	HttpParams,
} from '@angular/common/http';
import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { GetPlayersResult } from './getPlayersResult';

const PLAYERS_BASE_URL = 'https://localhost:5001/api/v1/Players';
export const DEFAULT_PAGE_SIZE = 13;

@Injectable({
	providedIn: 'root',
})
export class PlayersService {
	constructor(private httpClient: HttpClient) {}

	getPlayers(count = DEFAULT_PAGE_SIZE, offset = 0) {
		let params = new HttpParams();
		params = params.append('count', count.toString());
		params = params.append('offset', offset.toString());

		return this.httpClient
			.get<GetPlayersResult>(PLAYERS_BASE_URL, { params: params })
			.pipe(catchError(this.handleError));
	}

	private handleError(error: HttpErrorResponse) {
		return throwError(() => new Error(error.statusText));
	}
}
