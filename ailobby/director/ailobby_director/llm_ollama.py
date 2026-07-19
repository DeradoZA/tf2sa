"""Ollama backend: a local small model (default Qwen2.5-3B), $0, CPU-friendly.

Uses Ollama's native /api/chat with structured outputs (`format` = JSON schema)
so even a 3B model can't emit invalid JSON. Forces CPU (num_gpu 0) so the GPU
stays free for the TF2 client — see docs/local-model.md.
"""

from __future__ import annotations

import logging

import httpx

from .llm import (
    OUTPUT_SCHEMA,
    ChatLine,
    LlmClient,
    build_system,
    build_user,
    parse_chat_lines,
)

log = logging.getLogger(__name__)


class OllamaClient(LlmClient):
    def __init__(self, host: str, model: str, timeout_s: float) -> None:
        super().__init__()
        self._model = model
        # Local CPU inference is slower than the API; give the request real time
        # to finish (the strategist's stale-drop discards ones that were too slow
        # to still be useful). Floor at 20s so short outputs aren't cut off.
        self._client = httpx.AsyncClient(
            base_url=f"http://{host}", timeout=max(timeout_s, 20.0)
        )

    @property
    def label(self) -> str:
        return f"ollama:{self._model}"

    async def comms(
        self,
        personas_block: str,
        trigger: str,
        state_summary: str,
        react: bool = True,
        bot_names: list[str] | None = None,
    ) -> list[ChatLine]:
        resp = await self._client.post(
            "/api/chat",
            json={
                "model": self._model,
                "stream": False,
                # Keep the model resident so a call never pays a cold reload
                # (a reload would blow the strategist's stale-drop).
                "keep_alive": "30m",
                "format": OUTPUT_SCHEMA,  # structured output (Ollama >= 0.5)
                "messages": [
                    {"role": "system", "content": build_system(personas_block)},
                    {
                        "role": "user",
                        "content": build_user(trigger, state_summary, react, bot_names),
                    },
                ],
                "options": {
                    "temperature": 0.8,   # a little variety for persona flavour
                    "num_predict": 300,
                    "num_gpu": 0,         # CPU only — leave the GPU for the game
                },
            },
        )
        resp.raise_for_status()
        data = resp.json()
        self.total_calls += 1
        self.last_usage = {
            "input": data.get("prompt_eval_count", 0),
            "output": data.get("eval_count", 0),
        }
        text = data.get("message", {}).get("content")
        return parse_chat_lines(text)

    async def aclose(self) -> None:
        await self._client.aclose()
