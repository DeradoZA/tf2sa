"""Trigger policy: decides WHEN to ask the LLM for bot chat.

Event-driven calls (coalesced within 300ms) + a jittered periodic call, with a
global minimum interval and a staleness drop — reactions that arrive late read
as bot-like, so they're discarded.
"""

from __future__ import annotations

import asyncio
import logging
import random
import time

from . import events as ev
from .config import Settings
from .dispatcher import Dispatcher
from .llm import LlmClient
from .personas import PersonaManager
from .state import MatchState
from .stats import StatsAccumulator

log = logging.getLogger(__name__)


def describe_trigger(event: ev.Event, state: MatchState) -> str | None:
    """Return a human-readable trigger description, or None if not a trigger."""
    match event:
        case ev.MedicDeathEvent(victim=v, attacker=a, had_uber=had):
            uber_s = " while holding FULL UBER" if had else ""
            return f"Medic {v.name} [{v.team}] just died{uber_s} (killed by {a.name})"
        case ev.ChargeDeployedEvent(player=p):
            return f"{p.name} [{p.team}] just popped uber"
        case ev.PointCapturedEvent(team=t):
            return f"{t} just captured the point"
        case ev.RoundWinEvent(winner=w):
            return f"{w} just won the round"
        case ev.RoundStartEvent():
            return "A new round just started"
        case ev.ChatEvent(player=p, text=t) if not p.is_bot:
            return f"Human player {p.name} just said: {t!r} — consider replying"
        case ev.KillEvent(attacker=a, victim=v):
            streak = state.streak(a.userid)
            if state.last_kill_was_first_blood:
                return f"First blood of the round: {a.name} [{a.team}] killed {v.name}"
            if streak >= 3:
                return f"{a.name} [{a.team}] is on a {streak}-kill streak (just killed {v.name})"
            return None
        case _:
            return None


class Strategist:
    def __init__(
        self,
        bus: asyncio.Queue[ev.Event],
        state: MatchState,
        personas: PersonaManager,
        llm: LlmClient,
        dispatcher: Dispatcher,
        settings: Settings,
    ) -> None:
        self._bus = bus
        self._state = state
        self._personas = personas
        self._llm = llm
        self._dispatcher = dispatcher
        self._s = settings
        self._stats = StatsAccumulator()
        self._pending: list[str] = []
        self._coalesce_task: asyncio.Task | None = None
        self._last_call_at = 0.0

    async def run(self) -> None:
        periodic = asyncio.create_task(self._periodic_loop())
        try:
            while True:
                event = await self._bus.get()
                self._state.apply(event)
                self._stats.apply(event)
                if isinstance(event, ev.RawEvent):
                    if event.line:
                        log.debug("raw: %s", event.line)
                    continue
                if isinstance(event, (ev.RoundWinEvent, ev.RoundStalemateEvent)):
                    log.info("ROUND OVER — %s", self._stats.human_report())
                    self._stats.reset_round()
                trigger = describe_trigger(event, self._state)
                if trigger is not None:
                    self._queue_trigger(trigger)
        finally:
            periodic.cancel()

    # ---- event-driven path ----------------------------------------------

    def _queue_trigger(self, trigger: str) -> None:
        self._pending.append(trigger)
        if self._coalesce_task is None or self._coalesce_task.done():
            self._coalesce_task = asyncio.create_task(self._fire_after_coalesce())

    async def _fire_after_coalesce(self) -> None:
        await asyncio.sleep(self._s.coalesce_ms / 1000.0)
        triggers, self._pending = self._pending, []
        if not triggers:
            return
        await self._call_llm("; ".join(triggers), react=True)

    # ---- periodic path ---------------------------------------------------

    async def _periodic_loop(self) -> None:
        while True:
            await asyncio.sleep(random.uniform(self._s.periodic_min_s, self._s.periodic_max_s))
            if time.monotonic() - self._last_call_at < self._s.periodic_suppress_s:
                continue
            if not self._state.bots():
                continue
            await self._call_llm(
                "Nothing dramatic happened recently — optional ambient moment.",
                react=False,
            )

    # ---- shared ----------------------------------------------------------

    async def _call_llm(self, trigger: str, react: bool) -> None:
        now = time.monotonic()
        if now - self._last_call_at < self._s.min_call_interval_s:
            log.debug("rate limit: skipping call for trigger %r", trigger)
            return
        self._last_call_at = now

        bot_names = [b.name for b in self._state.bots()]
        self._personas.sync(bot_names)
        personas_block = self._personas.render_for_prompt()
        if not personas_block:
            return

        started = time.monotonic()
        try:
            lines = await self._llm.comms(
                personas_block, trigger, self._state.summary(), react, bot_names
            )
        except Exception as exc:
            log.warning("LLM call failed (%s): %s", type(exc).__name__, exc)
            return

        latency = time.monotonic() - started
        if latency > self._s.stale_drop_s:
            log.info("dropping stale LLM response (%.1fs)", latency)
            return

        log.info(
            "llm call #%d %.2fs trigger=%r -> %d line(s) usage=%s",
            self._llm.total_calls, latency, trigger[:60], len(lines), self._llm.last_usage,
        )
        if lines:
            asyncio.create_task(self._dispatcher.deliver(lines))
