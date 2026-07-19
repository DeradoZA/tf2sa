"""Per-player kills / DPM accumulator — the feedback loop for the goal metric.

Goal: cap a normal human at <30 kills / <8000 DPM by making bots that don't
feed. This measures whether we're there. Fed the same event stream as the
strategist; report at round end or on demand.

DPM = damage / minutes-of-round-time. Damage requires PlayerDamageEvent, which
needs supstats2 or a damage hook in ailobby.sp (see events.py). Kills work with
the stock stream alone, so a kill report is always available.
"""

from __future__ import annotations

from dataclasses import dataclass, field
from datetime import datetime

from . import events as ev


@dataclass
class PlayerStats:
    name: str
    is_bot: bool
    kills: int = 0
    deaths: int = 0
    damage: int = 0


@dataclass
class StatsAccumulator:
    players: dict[int, PlayerStats] = field(default_factory=dict)  # by userid
    round_start: datetime | None = None
    round_end: datetime | None = None

    def apply(self, event: ev.Event) -> None:
        match event:
            case ev.RoundStartEvent(ts=ts):
                self.round_start = ts
                self.round_end = None
            case ev.RoundWinEvent(ts=ts) | ev.RoundStalemateEvent(ts=ts):
                self.round_end = ts
            case ev.KillEvent(attacker=a, victim=v):
                self._for(a).kills += 1
                self._for(v).deaths += 1
            case ev.SuicideEvent(player=p):
                self._for(p).deaths += 1
            case ev.PlayerDamageEvent(player=p, damage=d):
                self._for(p).damage += d
            case _:
                pass

    def _for(self, p: ev.Player) -> PlayerStats:
        ps = self.players.get(p.userid)
        if ps is None:
            ps = PlayerStats(name=p.name, is_bot=p.is_bot)
            self.players[p.userid] = ps
        else:
            ps.name = p.name
        return ps

    def elapsed_minutes(self, now: datetime | None = None) -> float:
        if self.round_start is None:
            return 0.0
        end = self.round_end or now or datetime.now()
        secs = max((end - self.round_start).total_seconds(), 1.0)
        return secs / 60.0

    def dpm(self, userid: int, now: datetime | None = None) -> float:
        ps = self.players.get(userid)
        if ps is None:
            return 0.0
        mins = self.elapsed_minutes(now)
        return ps.damage / mins if mins > 0 else 0.0

    def human_report(self, now: datetime | None = None) -> str:
        """One line per human — the numbers we're trying to cap."""
        humans = [(uid, ps) for uid, ps in self.players.items() if not ps.is_bot]
        if not humans:
            return "No humans this round."
        has_damage = any(ps.damage > 0 for _uid, ps in humans)
        lines = ["Human performance (goal: cap < 30 kills / < 8000 DPM):"]
        for uid, ps in sorted(humans, key=lambda kv: kv[1].kills, reverse=True):
            dpm_s = f"{self.dpm(uid, now):.0f} DPM" if has_damage else "DPM n/a (no damage events)"
            flag = ""
            if ps.kills >= 30:
                flag += " [!kills]"
            if has_damage and self.dpm(uid, now) >= 8000:
                flag += " [!dpm]"
            lines.append(f"  {ps.name}: {ps.kills}k/{ps.deaths}d, {dpm_s}{flag}")
        if not has_damage:
            lines.append(
                "  (DPM unavailable: install supstats2 or add a damage hook to ailobby.sp)"
            )
        return "\n".join(lines)

    def reset_round(self) -> None:
        for ps in self.players.values():
            ps.kills = ps.deaths = ps.damage = 0
        self.round_start = self.round_end = None
