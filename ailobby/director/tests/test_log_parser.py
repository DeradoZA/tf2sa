"""Golden tests for the HL log parser.

Fixture lines are synthesised from the documented HL log format. At milestone
M2, capture real lines from the running server into fixtures/ and extend these.
"""

from pathlib import Path

from ailobby_director import events as ev
from ailobby_director.log_listener import extract_line
from ailobby_director.log_parser import parse_line

FIXTURES = Path(__file__).parent / "fixtures"


def load_fixture_events() -> list[ev.Event]:
    lines = (FIXTURES / "sample.log").read_text().strip().splitlines()
    return [parse_line(line) for line in lines]


def test_fixture_parses_without_error():
    parsed = load_fixture_events()
    assert len(parsed) == 15


def test_kill_event():
    e = load_fixture_events()[1]
    assert isinstance(e, ev.KillEvent)
    assert e.attacker.name == "Scout"
    assert e.attacker.is_bot
    assert e.victim.team == "Blue"
    assert e.weapon == "scattergun"
    assert e.crit is False


def test_crit_kill():
    e = load_fixture_events()[2]
    assert isinstance(e, ev.KillEvent)
    assert e.crit is True


def test_human_chat():
    e = load_fixture_events()[3]
    assert isinstance(e, ev.ChatEvent)
    assert not e.player.is_bot
    assert e.text == "howzit team"
    assert e.team_chat is False

    team = load_fixture_events()[4]
    assert isinstance(team, ev.ChatEvent)
    assert team.team_chat is True


def test_point_captured():
    e = load_fixture_events()[5]
    assert isinstance(e, ev.PointCapturedEvent)
    assert e.team == "Blue"
    assert e.cp == 0


def test_medic_death_with_uber():
    e = load_fixture_events()[6]
    assert isinstance(e, ev.MedicDeathEvent)
    assert e.victim.name == "Doc"
    assert e.healing == 1873
    assert e.had_uber is True


def test_charge_deployed_and_uber_status():
    charge = load_fixture_events()[7]
    assert isinstance(charge, ev.ChargeDeployedEvent)

    uber = load_fixture_events()[8]
    assert isinstance(uber, ev.UberStatusEvent)
    assert uber.charge == 0.62


def test_round_lifecycle():
    parsed = load_fixture_events()
    assert isinstance(parsed[0], ev.RoundStartEvent)
    win = parsed[13]
    assert isinstance(win, ev.RoundWinEvent)
    assert win.winner == "Blue"


def test_unknown_line_is_raw_not_fatal():
    e = load_fixture_events()[14]
    assert isinstance(e, ev.RawEvent)
    assert "server_cvar" in e.line


def test_garbage_line_is_raw():
    e = parse_line("complete nonsense")
    assert isinstance(e, ev.RawEvent)


# ---- UDP payload extraction -------------------------------------------------

MAGIC = b"\xff\xff\xff\xff"


def test_extract_standard_packet():
    line = extract_line(MAGIC + b'RL 07/19/2026 - 12:00:01: World triggered "Round_Start"\n\x00', "")
    assert line is not None and line.startswith("L 07/19/2026")


def test_extract_secret_packet():
    payload = MAGIC + b'Ss3cretL 07/19/2026 - 12:00:01: World triggered "Round_Start"\n\x00'
    assert extract_line(payload, "s3cret") is not None
    assert extract_line(payload, "wrong") is None


def test_unsecured_packet_dropped_when_secret_configured():
    payload = MAGIC + b'RL 07/19/2026 - 12:00:01: World triggered "Round_Start"\n\x00'
    assert extract_line(payload, "s3cret") is None


def test_non_magic_packet_ignored():
    assert extract_line(b"garbage", "") is None
