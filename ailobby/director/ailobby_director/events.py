"""Typed events parsed from the srcds remote log stream."""

from __future__ import annotations

from dataclasses import dataclass, field
from datetime import datetime


@dataclass(frozen=True)
class Player:
    name: str
    userid: int
    steamid: str
    team: str  # "Red" | "Blue" | "Spectator" | "Unassigned"

    @property
    def is_bot(self) -> bool:
        return self.steamid == "BOT"


@dataclass(frozen=True)
class Event:
    ts: datetime


@dataclass(frozen=True)
class KillEvent(Event):
    attacker: Player
    victim: Player
    weapon: str
    crit: bool


@dataclass(frozen=True)
class SuicideEvent(Event):
    player: Player
    weapon: str


@dataclass(frozen=True)
class PlayerDamageEvent(Event):
    """supstats2 'triggered "damage"' line (or our plugin's equivalent).

    Not emitted by stock srcds — requires the supstats2 SourceMod plugin or a
    damage hook in ailobby.sp. Kills are available without it; DPM needs it.
    """

    player: Player
    damage: int


@dataclass(frozen=True)
class ChatEvent(Event):
    player: Player
    text: str
    team_chat: bool


@dataclass(frozen=True)
class PointCapturedEvent(Event):
    team: str
    cp: int
    cpname: str


@dataclass(frozen=True)
class RoundStartEvent(Event):
    pass


@dataclass(frozen=True)
class RoundWinEvent(Event):
    winner: str


@dataclass(frozen=True)
class RoundStalemateEvent(Event):
    pass


@dataclass(frozen=True)
class RoleChangeEvent(Event):
    player: Player
    role: str


@dataclass(frozen=True)
class MedicDeathEvent(Event):
    attacker: Player
    victim: Player
    healing: int
    had_uber: bool


@dataclass(frozen=True)
class ChargeDeployedEvent(Event):
    player: Player


@dataclass(frozen=True)
class UberStatusEvent(Event):
    """From our plugin's LogToGame timer — medic uber charge 0.0..1.0."""

    player: Player
    charge: float


@dataclass(frozen=True)
class JoinedTeamEvent(Event):
    player: Player
    team: str


@dataclass(frozen=True)
class DisconnectEvent(Event):
    player: Player
    reason: str


@dataclass(frozen=True)
class NameChangeEvent(Event):
    player: Player
    new_name: str


@dataclass(frozen=True)
class RawEvent(Event):
    """Any line we don't recognise — logged, never fatal."""

    line: str = field(default="")
