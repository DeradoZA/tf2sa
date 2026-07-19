from datetime import datetime, timedelta

from ailobby_director import events as ev
from ailobby_director.stats import StatsAccumulator

T0 = datetime(2026, 7, 19, 12, 0, 0)


def human(name: str, uid: int, team: str = "Red") -> ev.Player:
    return ev.Player(name=name, userid=uid, steamid="[U:1:1]", team=team)


def bot(name: str, uid: int, team: str = "Blue") -> ev.Player:
    return ev.Player(name=name, userid=uid, steamid="BOT", team=team)


def test_kills_and_deaths_tracked():
    acc = StatsAccumulator()
    acc.apply(ev.RoundStartEvent(ts=T0))
    acc.apply(ev.KillEvent(ts=T0, attacker=human("Adam", 2), victim=bot("B", 5), weapon="x", crit=False))
    acc.apply(ev.KillEvent(ts=T0, attacker=bot("B", 5), victim=human("Adam", 2), weapon="x", crit=False))
    assert acc.players[2].kills == 1
    assert acc.players[2].deaths == 1
    assert acc.players[5].kills == 1


def test_dpm_computed_over_round_time():
    acc = StatsAccumulator()
    acc.apply(ev.RoundStartEvent(ts=T0))
    # 8000 damage over a 2-minute round = 4000 DPM
    acc.apply(ev.PlayerDamageEvent(ts=T0, player=human("Adam", 2), damage=8000))
    acc.apply(ev.RoundWinEvent(ts=T0 + timedelta(minutes=2), winner="Red"))
    assert round(acc.dpm(2)) == 4000


def test_report_flags_when_over_cap():
    acc = StatsAccumulator()
    acc.apply(ev.RoundStartEvent(ts=T0))
    for i in range(30):
        acc.apply(ev.KillEvent(ts=T0, attacker=human("Farmer", 2), victim=bot("B", 100 + i), weapon="x", crit=False))
    acc.apply(ev.PlayerDamageEvent(ts=T0, player=human("Farmer", 2), damage=9000))
    acc.apply(ev.RoundWinEvent(ts=T0 + timedelta(minutes=1), winner="Red"))
    report = acc.human_report()
    assert "[!kills]" in report
    assert "[!dpm]" in report


def test_report_handles_no_damage_events():
    acc = StatsAccumulator()
    acc.apply(ev.RoundStartEvent(ts=T0))
    acc.apply(ev.KillEvent(ts=T0, attacker=human("Adam", 2), victim=bot("B", 5), weapon="x", crit=False))
    report = acc.human_report()
    assert "DPM n/a" in report
    assert "supstats2" in report


def test_bots_excluded_from_human_report():
    acc = StatsAccumulator()
    acc.apply(ev.RoundStartEvent(ts=T0))
    acc.apply(ev.KillEvent(ts=T0, attacker=bot("B", 5), victim=bot("C", 6), weapon="x", crit=False))
    assert "No humans" in acc.human_report()
