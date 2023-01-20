import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Player } from 'src/app/models/player';

const PLAYERS_BASE_URL = 'https://localhost:5001/api/v1/players5';

@Injectable({
	providedIn: 'root',
})
export class PlayersService {
	constructor(private httpClient: HttpClient) {}

	getPlayers() {
		return this.httpClient
			.get<Player[]>(PLAYERS_BASE_URL)
			.pipe(catchError(this.handleError));
	}

	private handleError(error: HttpErrorResponse) {
		return throwError(() => new Error(error.statusText));
	}
}
