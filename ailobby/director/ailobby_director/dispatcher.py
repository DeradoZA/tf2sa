"""Chat output: takes LLM-decided lines and delivers them in-game via RCON
`ai_say`, with human-feeling pacing and hard anti-spam limits.

This is also the injection surface: LLM text goes into a server command, so it
is sanitised aggressively (quotes/semicolons/newlines stripped, length clamp).
"""

from __future__ import annotations

import asyncio
import logging
import re
import time
from collections import deque

from .config import Settings
from .llm import ChatLine
from .rcon_client import RconClient
from .state import MatchState

log = logging.getLogger(__name__)

# Strip only the genuinely dangerous chars for the RCON `ai_say <uid> <text>`
# path: double-quote and semicolon (console arg/command injection), newlines,
# and the literal "rcon". Apostrophes and % are safe and common in chat, so we
# keep them (previously they mangled "let's" -> "let s").
_FORBIDDEN = re.compile(r'["\n\r;]|rcon', re.IGNORECASE)


def sanitize(text: str, max_len: int) -> str:
    text = _FORBIDDEN.sub(" ", text)
    text = " ".join(text.split())  # collapse whitespace
    return text[:max_len].strip()


class Dispatcher:
    def __init__(self, rcon: RconClient, state: MatchState, settings: Settings) -> None:
        self._rcon = rcon
        self._state = state
        self._s = settings
        self._last_spoke: dict[str, float] = {}  # bot name -> monotonic time
        self._recent_sends: deque[float] = deque()

    async def deliver(self, lines: list[ChatLine]) -> None:
        """Deliver a batch of chat lines with delays, cooldowns and rate limits."""
        for line in lines:
            now = time.monotonic()

            # Per-bot cooldown
            if now - self._last_spoke.get(line.bot, 0.0) < self._s.per_bot_cooldown_s:
                log.debug("cooldown: dropping line from %s", line.bot)
                continue

            # Team-wide burst limit
            while self._recent_sends and now - self._recent_sends[0] > self._s.team_burst_window_s:
                self._recent_sends.popleft()
            if len(self._recent_sends) >= self._s.team_burst_limit:
                log.debug("burst limit: dropping line from %s", line.bot)
                continue

            text = sanitize(line.text, self._s.max_chat_len)
            if not text:
                continue

            userid = self._state.bot_userid_by_name(line.bot)
            if userid is None:
                log.debug("no bot named %r in roster; dropping line", line.bot)
                continue

            # Stagger + rough typing time so delivery feels human.
            delay = min(max(line.delay_ms, 0), 2500) / 1000.0
            typing = min(len(text) * 0.06, 2.0)
            await asyncio.sleep(delay + typing)

            try:
                await self._rcon.execute(f"ai_say {userid} {text}")
            except Exception as exc:
                log.warning("failed to deliver chat line: %s", exc)
                continue

            sent_at = time.monotonic()
            self._last_spoke[line.bot] = sent_at
            self._recent_sends.append(sent_at)
            log.info("chat [%s]: %s", line.bot, text)
