"""Standalone persona-chat benchmark against the local Ollama backend.

Judges model quality/latency independent of the game. Run with the rig up:

    docker run --rm --network host -v "$PWD":/app -w /app python:3.12-slim \
      bash -c "pip install -q httpx pyyaml && python bench.py [model-tag]"

Default model: qwen2.5:3b-instruct (pass e.g. qwen2.5:1.5b-instruct to compare).
"""

import asyncio
import sys
import time

from ailobby_director.llm_ollama import OllamaClient

PERSONAS = """\
- "Scoot" — cocky fast-talking scout, celebrates kills, tilts loudly. Style: fast, caps sometimes, "bro". Chattiness: high.
- "Doc" — anxious medic, narrates danger, begs for cover. Style: frantic. Chattiness: medium.
- "Oom" — laid-back SA uncle, rugby/braai metaphors, slang (howzit, lekker, bru, eish). Chattiness: medium.
- "Ghost" — man of few words, only speaks for important info. Chattiness: low.
- "Tank" — hype engineer, announces buildings, rallies pushes. Chattiness: medium."""
NAMES = ["Scoot", "Doc", "Oom", "Ghost", "Tank"]
STATE = """\
Score: Red 1 - Blue 2. Point held by: Blue. Round active.
Red: Doc [medic], Scoot, Oom, Ghost, Tank
Uber: our Doc [Red] at 100%"""
CASES = [
    (True, "We just built full uber and have a player advantage"),
    (True, "Our medic Doc died holding full uber (killed by their scout)"),
    (True, "Human teammate said: 'yall bots are trash lol'"),
    (True, "Human teammate asked: 'do we push or hold?'"),
    (True, "We just lost the round 3-2"),
    (True, "Ghost got a 4-kill streak on their team"),
    (False, "Nothing notable is happening right now"),
]


async def main() -> None:
    model = sys.argv[1] if len(sys.argv) > 1 else "qwen2.5:3b-instruct"
    client = OllamaClient("127.0.0.1:11434", model, 25.0)
    total = 0.0
    for react, trigger in CASES:
        start = time.monotonic()
        lines = await client.comms(PERSONAS, trigger, STATE, react, NAMES)
        elapsed = time.monotonic() - start
        total += elapsed
        print(f"\n[{'REACT' if react else 'QUIET'}] {trigger}   ({elapsed:.1f}s)")
        for line in lines:
            print(f"    {line.bot}: {line.text}")
        if not lines:
            print("    (silent)")
    print(f"\n{model}: avg {total / len(CASES):.1f}s/call")
    await client.aclose()


asyncio.run(main())
