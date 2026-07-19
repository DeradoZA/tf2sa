# AI Lobby (PoC)

**Problem:** the SA TF2 community is small — games are hard to come by.
**Idea:** AI Lobbies — a game mode where bots feel human enough to practice
with and have fun. This proof of concept gives stock TFBots a **voice**: an AI
Director watches the live game and generates persona-attributed team chat
(uber calls, reactions to picks, light banter) via the Anthropic API.

No bot *control* yet — mechanics are stock TFBot. The bet is that game-aware
comms alone transforms the feel. Strategy control is the phase-2 follow-up.

```
srcds (cm2network/tf2:sourcemod, host net)
  --UDP log stream (logaddress_add 127.0.0.1:27100)--> director (Python)
  <--RCON: ai_say <userid> <text> (tiny SourceMod plugin)--
```

## Layout

| Path | What |
|---|---|
| `docker-compose.yml` (+`.local`/`.vps`) | One rig definition, two deployments |
| `server/cfg/` | srcds config: logging, bot quota, map |
| `plugin/ailobby.sp` | ~70-line SourceMod plugin: `ai_say` + uber telemetry |
| `director/` | Python 3.12 asyncio service: log parser → match state → LLM → RCON |
| `docs/` | architecture + runbooks (local WSL2, VPS) |

## Quickstart (local, WSL2)

```bash
cp .env.example .env            # set RCON_PASSWORD + ANTHROPIC_API_KEY
./plugin/build.sh               # compile ailobby.smx (containerised, no toolchain needed)
docker compose -p ailobby -f docker-compose.yml -f docker-compose.local.yml up -d
# first start downloads the TF2 dedicated server (~20 GB) via SteamCMD — be patient
```

Then from TF2 on Windows: `connect <WSL-IP>:27015` (or `127.0.0.1:27015` with
mirrored networking) — see [docs/runbook-local.md](docs/runbook-local.md).

## Milestones

- **M1** — server up, Windows client connects, 11 bots play viaduct
- **M2** — events streaming into the director (verified against real captures)
- **M3** — first LLM chat line attributed to a bot in-game (<2s after a medic pick)
- **M4** — personas feel coherent over a full match; ~$0.10–0.30/match

## Tests

```bash
cd director && python -m venv .venv && .venv/bin/pip install -e '.[dev]'
.venv/bin/pytest
```
