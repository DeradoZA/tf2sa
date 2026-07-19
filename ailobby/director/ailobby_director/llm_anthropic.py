"""Anthropic backend: claude-haiku-4-5 with cached system prompt + json_schema.

Highest quality, ~$0.15/match. See llm.py for the shared prompt/parse logic.
"""

from __future__ import annotations

import logging

from anthropic import AsyncAnthropic

from .llm import (
    OUTPUT_SCHEMA,
    ChatLine,
    LlmClient,
    build_system,
    build_user,
    parse_chat_lines,
)

log = logging.getLogger(__name__)


class AnthropicClient(LlmClient):
    def __init__(self, model: str, timeout_s: float) -> None:
        super().__init__()
        # No retries: a missed quip beats a late one.
        self._client = AsyncAnthropic(timeout=timeout_s, max_retries=0)
        self._model = model

    @property
    def label(self) -> str:
        return f"anthropic:{self._model}"

    async def comms(
        self,
        personas_block: str,
        trigger: str,
        state_summary: str,
        react: bool = True,
        bot_names: list[str] | None = None,
    ) -> list[ChatLine]:
        response = await self._client.messages.create(
            model=self._model,
            max_tokens=300,
            system=[
                {
                    "type": "text",
                    "text": build_system(personas_block),
                    # Below Haiku's 4096-token cache minimum today, so this is a
                    # no-op until the prompt grows — harmless, kept for when it does.
                    "cache_control": {"type": "ephemeral"},
                }
            ],
            output_config={"format": {"type": "json_schema", "schema": OUTPUT_SCHEMA}},
            messages=[
                {"role": "user", "content": build_user(trigger, state_summary, react, bot_names)}
            ],
        )
        self.total_calls += 1
        u = response.usage
        self.last_usage = {
            "input": u.input_tokens,
            "output": u.output_tokens,
            "cache_read": u.cache_read_input_tokens or 0,
            "cache_write": u.cache_creation_input_tokens or 0,
        }
        text = next((b.text for b in response.content if b.type == "text"), None)
        return parse_chat_lines(text)
