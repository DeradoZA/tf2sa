import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './components/pages/home/home.component';
import { StatsComponent } from './components/pages/stats/stats.component';

const routes: Routes = [
	{ path: HomeComponent.PATH, component: HomeComponent },
	{ path: StatsComponent.PATH, component: StatsComponent },
];

@NgModule({
	imports: [RouterModule.forRoot(routes)],
	exports: [RouterModule],
})
export class AppRoutingModule {}
