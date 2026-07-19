# Local LLM backend (Ollama)

The director has two interchangeable LLM backends, selected by `LLM_BACKEND`:

| Backend | Cost | Quality | Resources |
|---|---|---|---|
| `ollama` (default) | free | good-enough, flatter personas | CPU-only, ~3–4 GB RAM |
| `anthropic` | ~$0.15/match | best | none local (API) |

Only `llm_anthropic.py` / `llm_ollama.py` differ; the parser, state, strategist
and dispatcher are backend-agnostic. Switch by editing `LLM_BACKEND` in `.env`
and restarting the director — no rebuild.

## Why CPU, not GPU (on a 6 GB laptop)

The TF2 **client** you play on Windows needs the GPU to render, and a 6 GB card
(e.g. RTX 4050 Laptop) can't comfortably host both the game and a model. The
dedicated **server** is headless (no GPU). So the model runs **CPU-only**
(`num_gpu: 0` in `llm_ollama.py`) and the GPU stays 100% for the game. With a
14-core CPU + 32 GB RAM, a 3B model sips a few cores and ~3–4 GB RAM.

Trade-off: CPU inference is slower (~3–5 s for a short reply vs ~0.8 s on the
API), so the director's stale-drop and timeout are relaxed automatically when
`LLM_BACKEND=ollama` (see `config.py`).

## Setup

The `ollama` service is part of the compose rig. One-time model pull:

```bash
docker compose -p ailobby -f docker-compose.yml -f docker-compose.local.yml up -d ollama
docker exec ailobby-ollama-1 ollama pull qwen2.5:3b-instruct
```

Then bring up the director (defaults to the ollama backend):

```bash
docker compose -p ailobby -f docker-compose.yml -f docker-compose.local.yml up -d director
docker compose -p ailobby logs -f director
```

## Choosing a model (by quality vs speed)

`qwen2.5:3b-instruct` is the default — good persona adherence for its size and
solid at constrained JSON. Alternatives (set `OLLAMA_MODEL` + `ollama pull`):

| Model | ~RAM | Notes |
|---|---|---|
| `llama3.2:3b-instruct` | ~3 GB | slightly chattier, comparable |
| `qwen2.5:3b-instruct` | ~3 GB | **default** — balanced |
| `qwen2.5:7b-instruct` | ~5–6 GB | better voice, ~2× slower on CPU |

Structured output is enforced via Ollama's `format` = JSON schema, so even a 3B
model can't emit invalid JSON.

## Going back to the API for a quality A/B

```bash
# in .env:
LLM_BACKEND=anthropic
ANTHROPIC_API_KEY=sk-ant-...
# then:
docker compose -p ailobby up -d director
```

The director logs `backend=ollama:...` or `backend=anthropic:...` at startup and
per-call usage, so you can compare feel and cost side by side.
