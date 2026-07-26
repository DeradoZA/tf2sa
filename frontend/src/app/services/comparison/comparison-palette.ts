// Validated categorical palette (dataviz skill, dark mode, fixed slot order) for the
// player-comparison chart and its legend/chips. Run through scripts/validate_palette.js
// at surface #262626 — see progression-chart.component.ts for the validator invocation.
//
// Deliberately a plain constant with NO Angular or ngx-charts imports: the eagerly
// loaded stats pages (the compare-selection chips) need these colors too, and
// importing anything from the ngx-charts-using progression-chart module would pull
// ngx-charts/d3 back into the initial bundle — the exact regression fixed by making
// the player and compare pages lazy in the first place.
export const COMPARISON_PALETTE: readonly string[] = [
	'#3987e5', // blue
	'#d95926', // orange
	'#199e70', // aqua
	'#c98500', // yellow
	'#d55181', // magenta
	'#008300', // green
];
