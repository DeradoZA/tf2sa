import { Component } from '@angular/core';
import { ROUTES } from './routes';
@Component({
	selector: 'app-root',
	templateUrl: './app.component.html',
	styleUrls: ['./app.component.scss'],
})
export class AppComponent {
	readonly ROUTES = ROUTES;
}
