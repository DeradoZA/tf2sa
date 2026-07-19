from datetime import datetime

from ailobby_director import events as ev
from ailobby_director.dispatcher import sanitize
from ailobby_director.personas import PersonaManager
from ailobby_director.state import MatchState

TS = datetime(2026, 7, 19, 12, 0, 0)


def bot(name: str, uid: int, team: str = "Red") -> ev.Player:
    return ev.Player(name=name, userid=uid, steamid="BOT", team=team)


def test_state_tracks_streaks_and_first_blood():
    s = MatchState()
    s.apply(ev.RoundStartEvent(ts=TS))
    s.apply(ev.KillEvent(ts=TS, attacker=bot("A", 1), victim=bot("B", 2, "Blue"), weapon="x", crit=False))
    assert s.last_kill_was_first_blood
    s.apply(ev.KillEvent(ts=TS, attacker=bot("A", 1), victim=bot("C", 3, "Blue"), weapon="x", crit=False))
    assert not s.last_kill_was_first_blood
    assert s.streak(1) == 2
    s.apply(ev.KillEvent(ts=TS, attacker=bot("C", 3, "Blue"), victim=bot("A", 1), weapon="x", crit=False))
    assert s.streak(1) == 0


def test_state_roster_and_lookup():
    s = MatchState()
    s.apply(ev.ChatEvent(ts=TS, player=bot("Aimbot", 7, "Blue"), text="hi", team_chat=False))
    human = ev.Player(name="Adam", userid=2, steamid="[U:1:1]", team="Red")
    s.apply(ev.ChatEvent(ts=TS, player=human, text="yo", team_chat=False))
    assert s.bot_userid_by_name("Aimbot") == 7
    assert s.bot_userid_by_name("Adam") is None  # humans never addressable via ai_say
    assert len(s.humans()) == 1
    summary = s.summary()
    assert "Aimbot" in summary and "Adam (human)" in summary


def test_sanitize_strips_injection_chars():
    assert sanitize('say "hello"; rcon quit\nboom', 120) == "say hello quit boom"
    assert sanitize("a" * 300, 120) == "a" * 120


def test_sanitize_keeps_apostrophes_and_percent():
    # these are safe in the say path and common in chat — must survive
    assert sanitize("let's push, we're at 90% uber", 120) == "let's push, we're at 90% uber"


def test_persona_assignment_stable_and_reassigns():
    pm = PersonaManager()
    pm.sync(["Bot1", "Bot2"])
    first = pm.assignments()["Bot1"].label
    pm.sync(["Bot1", "Bot2", "Bot3"])
    assert pm.assignments()["Bot1"].label == first  # stable
    pm.sync(["Bot2", "Bot3"])  # Bot1 left; its persona is freed
    pm.sync(["Bot2", "Bot3", "Bot4"])
    assert len(pm.assignments()) == 3
    prompt = pm.render_for_prompt()
    assert '"Bot2"' in prompt
