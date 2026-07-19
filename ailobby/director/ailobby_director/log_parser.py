"""HL log line -> typed Event.

Reference format (same family logs.tf parses):
  L 07/19/2026 - 12:34:56: "Scout<3><BOT><Red>" killed "Soldier<4><BOT><Blue>" with "scattergun"

Unknown lines become RawEvent — logged upstream, never fatal. Fixture lines in
tests/fixtures/ are synthesised from the documented format; replace them with
real captures at milestone M2.
"""

from __future__ import annotations

import re
from datetime import datetime

from . import events as ev

_PLAYER = r'"(?P<{p}name>.+?)<(?P<{p}uid>\d+)><(?P<{p}sid>[^>]*)><(?P<{p}team>[^>]*)>"'


def _player(prefix: str) -> str:
    return _PLAYER.replace("{p}", prefix)


_PREFIX = re.compile(
    r"^L (?P<date>\d{2}/\d{2}/\d{4}) - (?P<time>\d{2}:\d{2}:\d{2}): (?P<msg>.*)$"
)

_PATTERNS: list[tuple[re.Pattern[str], str]] = [
    (re.compile(rf"^{_player('a')} killed {_player('v')} with \"(?P<weapon>[^\"]+)\"(?P<rest>.*)$"), "kill"),
    (re.compile(rf"^{_player('a')} committed suicide with \"(?P<weapon>[^\"]+)\""), "suicide"),
    (re.compile(rf"^{_player('a')} triggered \"damage\" \(damage \"(?P<damage>\d+)\"\)"), "damage"),
    (re.compile(rf"^{_player('a')} triggered \"medic_death\" against {_player('v')}(?P<rest>.*)$"), "medic_death"),
    (re.compile(rf"^{_player('a')} triggered \"chargedeployed\""), "chargedeployed"),
    (re.compile(rf"^{_player('a')} triggered \"ailobby_uber\" \(charge \"(?P<charge>[\d.]+)\"\)"), "ailobby_uber"),
    (re.compile(rf"^{_player('a')} say \"(?P<text>.*)\"$"), "say"),
    (re.compile(rf"^{_player('a')} say_team \"(?P<text>.*)\"$"), "say_team"),
    (re.compile(r"^Team \"(?P<team>\w+)\" triggered \"pointcaptured\" \(cp \"(?P<cp>\d+)\"\) \(cpname \"(?P<cpname>[^\"]*)\"\)(?P<rest>.*)$"), "pointcaptured"),
    (re.compile(r"^World triggered \"Round_Start\""), "round_start"),
    (re.compile(r"^World triggered \"Round_Win\" \(winner \"(?P<winner>\w+)\"\)"), "round_win"),
    (re.compile(r"^World triggered \"Round_Stalemate\""), "round_stalemate"),
    (re.compile(rf"^{_player('a')} changed role to \"(?P<role>\w+)\""), "role_change"),
    (re.compile(rf"^{_player('a')} joined team \"(?P<team>\w+)\""), "joined_team"),
    (re.compile(rf"^{_player('a')} disconnected(?: \(reason \"(?P<reason>[^\"]*)\"\))?"), "disconnect"),
    (re.compile(rf"^{_player('a')} changed name to \"(?P<newname>.*)\"$"), "name_change"),
]


def _mk_player(m: re.Match[str], prefix: str) -> ev.Player:
    return ev.Player(
        name=m.group(f"{prefix}name"),
        userid=int(m.group(f"{prefix}uid")),
        steamid=m.group(f"{prefix}sid"),
        team=m.group(f"{prefix}team"),
    )


def parse_line(line: str) -> ev.Event:
    """Parse one raw log line into a typed Event (RawEvent if unrecognised)."""
    pm = _PREFIX.match(line.strip())
    if pm is None:
        return ev.RawEvent(ts=datetime.now(), line=line)

    try:
        ts = datetime.strptime(f"{pm.group('date')} {pm.group('time')}", "%m/%d/%Y %H:%M:%S")
    except ValueError:
        ts = datetime.now()
    msg = pm.group("msg")

    for pattern, kind in _PATTERNS:
        m = pattern.match(msg)
        if m is None:
            continue
        if kind == "kill":
            return ev.KillEvent(
                ts=ts,
                attacker=_mk_player(m, "a"),
                victim=_mk_player(m, "v"),
                weapon=m.group("weapon"),
                crit="crit" in m.group("rest"),
            )
        if kind == "suicide":
            return ev.SuicideEvent(ts=ts, player=_mk_player(m, "a"), weapon=m.group("weapon"))
        if kind == "damage":
            return ev.PlayerDamageEvent(ts=ts, player=_mk_player(m, "a"), damage=int(m.group("damage")))
        if kind == "medic_death":
            rest = m.group("rest")
            healing_m = re.search(r'\(healing "(\d+)"\)', rest)
            uber_m = re.search(r'\(ubercharge "(\d+)"\)', rest)
            return ev.MedicDeathEvent(
                ts=ts,
                attacker=_mk_player(m, "a"),
                victim=_mk_player(m, "v"),
                healing=int(healing_m.group(1)) if healing_m else 0,
                had_uber=bool(uber_m and uber_m.group(1) == "1"),
            )
        if kind == "chargedeployed":
            return ev.ChargeDeployedEvent(ts=ts, player=_mk_player(m, "a"))
        if kind == "ailobby_uber":
            return ev.UberStatusEvent(ts=ts, player=_mk_player(m, "a"), charge=float(m.group("charge")))
        if kind in ("say", "say_team"):
            return ev.ChatEvent(
                ts=ts, player=_mk_player(m, "a"), text=m.group("text"), team_chat=kind == "say_team"
            )
        if kind == "pointcaptured":
            return ev.PointCapturedEvent(
                ts=ts, team=m.group("team"), cp=int(m.group("cp")), cpname=m.group("cpname")
            )
        if kind == "round_start":
            return ev.RoundStartEvent(ts=ts)
        if kind == "round_win":
            return ev.RoundWinEvent(ts=ts, winner=m.group("winner"))
        if kind == "round_stalemate":
            return ev.RoundStalemateEvent(ts=ts)
        if kind == "role_change":
            return ev.RoleChangeEvent(ts=ts, player=_mk_player(m, "a"), role=m.group("role"))
        if kind == "joined_team":
            return ev.JoinedTeamEvent(ts=ts, player=_mk_player(m, "a"), team=m.group("team"))
        if kind == "disconnect":
            return ev.DisconnectEvent(ts=ts, player=_mk_player(m, "a"), reason=m.group("reason") or "")
        if kind == "name_change":
            return ev.NameChangeEvent(ts=ts, player=_mk_player(m, "a"), new_name=m.group("newname"))

    return ev.RawEvent(ts=ts, line=msg)
