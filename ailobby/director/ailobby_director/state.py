"""Rolling match state — the director's memory of what's happening in-game.

Fed every parsed event; produces the compact text summary that goes into each
LLM prompt (target: well under 300 tokens).
"""

from __future__ import annotations

from collections import deque
from dataclasses import dataclass, field
from datetime import datetime

from . import events as ev


@dataclass
class RosterEntry:
    name: str
    userid: int
    steamid: str
    team: str
    role: str = ""

    @property
    def is_bot(self) -> bool:
        return self.steamid == "BOT"


@dataclass
class MatchState:
    roster: dict[int, RosterEntry] = field(default_factory=dict)  # by userid
    scores: dict[str, int] = field(default_factory=lambda: {"Red": 0, "Blue": 0})
    killfeed: deque[str] = field(default_factory=lambda: deque(maxlen=20))
    recent_chat: deque[str] = field(default_factory=lambda: deque(maxlen=15))
    streaks: dict[int, int] = field(default_factory=dict)  # userid -> current streak
    uber: dict[int, float] = field(default_factory=dict)  # medic userid -> charge
    point_owner: str = "neutral"
    round_active: bool = False
    first_blood_done: bool = False
    last_kill_was_first_blood: bool = False
    last_event_ts: datetime | None = None

    # ---- updates ---------------------------------------------------------

    def apply(self, event: ev.Event) -> None:
        self.last_event_ts = event.ts
        match event:
            case ev.KillEvent(attacker=a, victim=v, weapon=w, crit=crit):
                self._touch(a)
                self._touch(v)
                self.streaks[a.userid] = self.streaks.get(a.userid, 0) + 1
                self.streaks[v.userid] = 0
                self.uber.pop(v.userid, None)
                crit_s = " (crit)" if crit else ""
                self.killfeed.append(f"{a.name} [{a.team}] killed {v.name} [{v.team}] with {w}{crit_s}")
                self.last_kill_was_first_blood = not self.first_blood_done
                self.first_blood_done = True
            case ev.SuicideEvent(player=p):
                self._touch(p)
                self.streaks[p.userid] = 0
                self.killfeed.append(f"{p.name} [{p.team}] suicided")
            case ev.MedicDeathEvent(attacker=a, victim=v, had_uber=had):
                self._touch(a)
                self._touch(v)
                self.uber.pop(v.userid, None)
                note = " WITH FULL UBER (huge)" if had else ""
                self.killfeed.append(f"MEDIC DOWN: {v.name} [{v.team}] killed by {a.name}{note}")
            case ev.ChargeDeployedEvent(player=p):
                self._touch(p)
                self.uber[p.userid] = 0.0
                self.killfeed.append(f"{p.name} [{p.team}] USED UBER")
            case ev.UberStatusEvent(player=p, charge=c):
                self._touch(p)
                self.uber[p.userid] = c
            case ev.ChatEvent(player=p, text=t, team_chat=tc):
                self._touch(p)
                scope = "(team)" if tc else ""
                who = "BOT" if p.is_bot else "HUMAN"
                self.recent_chat.append(f"[{who}] {p.name} [{p.team}]{scope}: {t}")
            case ev.PointCapturedEvent(team=team):
                self.point_owner = team
                self.scores[team] = self.scores.get(team, 0)
                self.killfeed.append(f"{team} captured the point")
            case ev.RoundStartEvent():
                self.round_active = True
                self.first_blood_done = False
                self.point_owner = "neutral"
                self.streaks.clear()
                self.uber.clear()
            case ev.RoundWinEvent(winner=winner):
                self.round_active = False
                self.scores[winner] = self.scores.get(winner, 0) + 1
                self.killfeed.append(f"{winner} won the round")
            case ev.RoundStalemateEvent():
                self.round_active = False
            case ev.RoleChangeEvent(player=p, role=role):
                self._touch(p)
                self.roster[p.userid].role = role
            case ev.JoinedTeamEvent(player=p, team=team):
                self._touch(p)
                self.roster[p.userid].team = team
            case ev.NameChangeEvent(player=p, new_name=new_name):
                self._touch(p)
                self.roster[p.userid].name = new_name
            case ev.DisconnectEvent(player=p):
                self.roster.pop(p.userid, None)
                self.streaks.pop(p.userid, None)
                self.uber.pop(p.userid, None)
            case _:
                pass

    def _touch(self, p: ev.Player) -> None:
        entry = self.roster.get(p.userid)
        if entry is None:
            self.roster[p.userid] = RosterEntry(
                name=p.name, userid=p.userid, steamid=p.steamid, team=p.team
            )
        else:
            entry.name = p.name
            if p.team in ("Red", "Blue"):
                entry.team = p.team

    # ---- queries ---------------------------------------------------------

    def bots(self) -> list[RosterEntry]:
        return [r for r in self.roster.values() if r.is_bot and r.team in ("Red", "Blue")]

    def humans(self) -> list[RosterEntry]:
        return [r for r in self.roster.values() if not r.is_bot and r.team in ("Red", "Blue")]

    def bot_userid_by_name(self, name: str) -> int | None:
        for r in self.roster.values():
            if r.is_bot and r.name == name:
                return r.userid
        return None

    def streak(self, userid: int) -> int:
        return self.streaks.get(userid, 0)

    # ---- prompt rendering ------------------------------------------------

    def summary(self) -> str:
        lines: list[str] = []
        lines.append(
            f"Score: Red {self.scores.get('Red', 0)} - Blue {self.scores.get('Blue', 0)}. "
            f"Point held by: {self.point_owner}. Round {'active' if self.round_active else 'over'}."
        )
        for team in ("Red", "Blue"):
            members = [r for r in self.roster.values() if r.team == team]
            if members:
                names = ", ".join(
                    f"{r.name}{' (human)' if not r.is_bot else ''}"
                    f"{f' [{r.role}]' if r.role else ''}"
                    for r in members
                )
                lines.append(f"{team}: {names}")
        for uid, charge in self.uber.items():
            r = self.roster.get(uid)
            if r is not None:
                lines.append(f"Uber: {r.name} [{r.team}] at {int(charge * 100)}%")
        if self.killfeed:
            lines.append("Recent events (oldest first):")
            lines.extend(f"  {k}" for k in list(self.killfeed)[-8:])
        if self.recent_chat:
            lines.append("Recent chat (oldest first):")
            lines.extend(f"  {c}" for c in self.recent_chat)
        return "\n".join(lines)
