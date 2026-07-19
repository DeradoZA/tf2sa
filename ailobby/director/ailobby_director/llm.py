"""LLM comms: shared prompt/parse logic + a pluggable backend interface.

Two backends implement the same LlmClient contract:
  - AnthropicClient (llm_anthropic.py) — claude-haiku-4-5, best quality
  - OllamaClient    (llm_ollama.py)    — local small model, $0, CPU-friendly

Select via LLM_BACKEND env (see config.py). The strategist only sees LlmClient.
"""

from __future__ import annotations

import abc
import json
import logging
from dataclasses import dataclass

log = logging.getLogger(__name__)

# JSON schema for the structured output. Anthropic enforces it via
# output_config.format; Ollama enforces it via the `format` field.
OUTPUT_SCHEMA = {
    "type": "object",
    "properties": {
        "messages": {
            "type": "array",
            "items": {
                "type": "object",
                "properties": {
                    "bot": {"type": "string"},
                    "delay_ms": {"type": "integer"},
                    "text": {"type": "string"},
                },
                "required": ["bot", "delay_ms", "text"],
                "additionalProperties": False,
            },
        }
    },
    "required": ["messages"],
    "additionalProperties": False,
}

SYSTEM_TEMPLATE = """\
You are the hidden director of the bot players in a casual Team Fortress 2 \
match (koth_viaduct, king-of-the-hill: hold the central point to win). You \
write their in-game text chat.

Voice rules:
- NEVER reveal or hint the bots are AI or scripted. They are just players.
- Only use info a player in the match could see (it is all in the game state).
- Each line <= 80 chars, casual game-chat register, mostly lowercase. No \
roleplay asterisks, no emoji spam.
- At most 3 lines, each from a DIFFERENT bot listed below, matching that bot's \
persona and chattiness. Low-chattiness bots speak rarely.
- SPREAD lines across the roster over time. Prefer a bot who has NOT spoken in \
the recent chat log; never let one bot dominate the conversation. The chattiest \
bot is not always the right speaker — pick whoever fits the moment.
- Do not repeat or closely paraphrase anything already in the recent chat log.
- delay_ms (0-2500) staggers the lines so bots don't all talk at once.

When to talk (IMPORTANT):
- You will be told whether this is a REACT moment or a QUIET moment.
- REACT = something just happened (a pick, an uber, a cap, a round end, or a \
human said something). On a REACT moment at least ONE bot SHOULD speak — reply \
to humans naturally, call out ubers and picks. Do NOT return an empty list on a \
REACT moment unless a bot just said the exact same thing.
- QUIET = nothing notable. Usually return {{"messages": []}}; occasionally one \
chatty bot makes small talk.

The bots on the server and their personas:
{personas}

Examples of good REACT output (the bot names below are illustrative — only ever \
use the actual bot names listed above):
- Our medic died holding full uber ->
{{"messages":[{{"bot":"Scoot","delay_ms":300,"text":"MED DOWN with full uber?? nooo"}},{{"bot":"Oom","delay_ms":1400,"text":"eish. regroup boys, we go again"}}]}}
- A human teammate said "howzit lads" ->
{{"messages":[{{"bot":"Oom","delay_ms":500,"text":"howzit bru, get on the point"}}]}}
- We have uber advantage ->
{{"messages":[{{"bot":"Scoot","delay_ms":200,"text":"we got uber, push mid together"}}]}}
"""

USER_TEMPLATE = """\
This is a {mode}

TRIGGER: {trigger}

CURRENT GAME STATE:
{state}
{names}
Output the JSON now."""


@dataclass(frozen=True)
class ChatLine:
    bot: str
    delay_ms: int
    text: str


def build_system(personas_block: str) -> str:
    return SYSTEM_TEMPLATE.format(personas=personas_block)


def build_user(
    trigger: str, state_summary: str, react: bool = True, bot_names: list[str] | None = None
) -> str:
    mode = (
        "REACT moment — at least one bot should respond."
        if react
        else "QUIET moment — usually say nothing, only rarely a small remark."
    )
    names = ", ".join(f'"{n}"' for n in (bot_names or []))
    constraint = (
        f"\nThe \"bot\" field MUST be EXACTLY one of these current bot names "
        f"(never invent a name, never use example names): {names}\n"
        if names
        else ""
    )
    return USER_TEMPLATE.format(mode=mode, trigger=trigger, state=state_summary, names=constraint)


def parse_chat_lines(text: str | None) -> list[ChatLine]:
    """Parse a model's JSON response into ChatLines. Tolerant of junk/None."""
    if not text:
        return []
    text = text.strip()
    # Some local models wrap JSON in ```json fences despite instructions.
    if text.startswith("```"):
        text = text.strip("`")
        if text.lstrip().startswith("json"):
            text = text.lstrip()[4:]
    try:
        data = json.loads(text)
    except json.JSONDecodeError:
        log.warning("LLM returned non-JSON: %.120s", text)
        return []
    out: list[ChatLine] = []
    for m in data.get("messages", []):
        try:
            out.append(ChatLine(bot=str(m["bot"]), delay_ms=int(m["delay_ms"]), text=str(m["text"])))
        except (KeyError, TypeError, ValueError):
            continue
    return out


class LlmClient(abc.ABC):
    """Backend contract. Implementations set last_usage + total_calls."""

    def __init__(self) -> None:
        self.total_calls = 0
        self.last_usage: dict[str, int] = {}

    @abc.abstractmethod
    async def comms(
        self,
        personas_block: str,
        trigger: str,
        state_summary: str,
        react: bool = True,
        bot_names: list[str] | None = None,
    ) -> list[ChatLine]:
        """One comms decision. Raises on transport errors — caller drops them."""

    @property
    @abc.abstractmethod
    def label(self) -> str:
        """Short human label for logging (e.g. 'anthropic:claude-haiku-4-5')."""


def make_llm(settings) -> LlmClient:  # noqa: ANN001 (avoid config import cycle)
    """Factory: build the backend named by settings.backend."""
    if settings.backend == "anthropic":
        from .llm_anthropic import AnthropicClient

        return AnthropicClient(settings.model, settings.llm_timeout_s)
    if settings.backend == "ollama":
        from .llm_ollama import OllamaClient

        return OllamaClient(settings.ollama_host, settings.ollama_model, settings.llm_timeout_s)
    raise ValueError(f"unknown LLM_BACKEND: {settings.backend!r} (use 'ollama' or 'anthropic')")
