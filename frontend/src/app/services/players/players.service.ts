import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { Player } from 'src/app/models/player';
import { HttpHeaders } from '@angular/common/http';

const PLAYERS_BASE_URL = 'https://localhost:5001/api/v1/players';
const httpOptions = {
	headers: new HttpHeaders({
		'Content-Type': 'application/json',
		'Access-Control-Allow-Origin': '*',
	}),
};

@Injectable({
	providedIn: 'root',
})
export class PlayersService {
	constructor(private httpClient: HttpClient) {}

	getPlayers() {
		return this.httpClient.get<Player[]>(PLAYERS_BASE_URL, httpOptions);
	}
}
