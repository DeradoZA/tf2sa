import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { GetPlayersResult } from './getPlayersResult';

const PLAYERS_BASE_URL = 'https://localhost:5001/api/v1/Players';

@Injectable({
	providedIn: 'root',
})
export class PlayersService {
	constructor(private httpClient: HttpClient) {}

	getPlayers() {
		return this.httpClient
			.get<GetPlayersResult>(PLAYERS_BASE_URL)
			.pipe(catchError(this.handleError));
	}

	private handleError(error: HttpErrorResponse) {
		return throwError(() => new Error(error.statusText));
	}
}
