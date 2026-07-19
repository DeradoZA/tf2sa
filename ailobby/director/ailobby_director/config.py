"""Environment-driven settings for the AI Director."""

from __future__ import annotations

import os
from dataclasses import dataclass


@dataclass(frozen=True)
class Settings:
    backend: str          # "ollama" (local, default) | "anthropic" (API)
    model: str            # anthropic model id
    ollama_host: str      # host:port for the Ollama server
    ollama_model: str     # local model tag
    rcon_host: str
    rcon_port: int
    rcon_password: str
    log_listen_port: int
    log_secret: str

    # Tunables (defaults per design; env-overridable for experimentation)
    coalesce_ms: int = 300          # merge trigger events arriving within this window
    min_call_interval_s: float = 2.0
    periodic_min_s: float = 45.0    # jittered "what's happening" call window
    periodic_max_s: float = 90.0
    periodic_suppress_s: float = 20.0  # skip periodic if an event call fired recently
    stale_drop_s: float = 4.0       # drop LLM responses older than this
    per_bot_cooldown_s: float = 8.0
    team_burst_limit: int = 3       # max lines per team_burst_window_s
    team_burst_window_s: float = 10.0
    llm_timeout_s: float = 4.0
    max_chat_len: int = 120

    @staticmethod
    def from_env() -> "Settings":
        backend = os.environ.get("LLM_BACKEND", "ollama").lower()
        # Local CPU inference competes with srcds' 12-bot AI for cores, so a
        # warm call can still take ~10s during a full match — relax the stale
        # threshold well past that so good responses aren't discarded (they land
        # a few seconds late, which reads as a slightly slow player, not a bug).
        # The API backend has no local CPU cost, so it stays tight.
        is_local = backend == "ollama"
        stale = float(os.environ.get("STALE_DROP_S", "15.0" if is_local else "4.0"))
        timeout = float(os.environ.get("LLM_TIMEOUT_S", "20.0" if is_local else "4.0"))
        return Settings(
            backend=backend,
            model=os.environ.get("AILOBBY_MODEL", "claude-haiku-4-5"),
            ollama_host=os.environ.get("OLLAMA_HOST", "127.0.0.1:11434"),
            ollama_model=os.environ.get("OLLAMA_MODEL", "qwen2.5:3b-instruct"),
            rcon_host=os.environ.get("RCON_HOST", "127.0.0.1"),
            rcon_port=int(os.environ.get("RCON_PORT", "27015")),
            rcon_password=os.environ["RCON_PASSWORD"],
            log_listen_port=int(os.environ.get("LOG_LISTEN_PORT", "27100")),
            log_secret=os.environ.get("LOG_SECRET", ""),
            stale_drop_s=stale,
            llm_timeout_s=timeout,
        )
