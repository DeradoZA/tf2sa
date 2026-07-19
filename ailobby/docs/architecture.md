# AI Lobby — Architecture

## The layered-bot insight

| Layer | Timescale | Owner |
|---|---|---|
| Mechanics (aim, movement) | ~15ms ticks | Stock TFBot — LLM latency is 100–1000× too slow |
| Strategy (hold/push/sac) | 3–10s | LLM (phase 2, not in PoC) |
| Communication (calls, banter) | event-driven | **LLM — this PoC** |

What breaks the "human" illusion isn't mediocre aim — it's silence and
incoherence: no uber calls, no reaction to a medic pick. Comms is the cheapest
layer to build and the biggest jump in feel.

## Bridge decision (Option C — hybrid)

Considered:

- **A: logaddress + RCON `say`** — zero SourcePawn, but chat renders as
  "Console", killing attribution.
- **B: SourceMod plugin + HTTP extension (ripext)** — richest, but large Pawn
  surface, extension ABI risk, threading in Pawn.
- **C (chosen): UDP log stream in, tiny plugin for chat out** — the log stream
  already carries kills/chat/captures/rounds in the documented HL format (the
  same family logs.tf parses — familiar territory for this repo). The plugin
  does only what RCON can't: `ai_say <userid> <msg>` via
  `FakeClientCommandEx(client, "say %s", msg)` for attributed chat, plus a
  3s timer logging medic `m_flChargeLevel` into the same stream.

No HTTP/socket extensions anywhere; the 32-bit `cm2network/tf2:sourcemod`
image keeps SourceMod ABI stable.

## Director data flow

```
UDP :27100 → log_listener → log_parser (regex table; unknown → RawEvent)
  → bus → strategist
             ├─ state.apply(event)          MatchState: roster, scores, killfeed,
             │                              streaks, uber %, recent chat
             ├─ trigger? (medic death, uber, capture, round, human chat,
             │            3+ streak, first blood; coalesced 300ms)
             ├─ periodic ambient call (jittered 45–90s, suppressed near events)
             └─ LLM (claude-haiku-4-5, json_schema output, 4s timeout,
                     no retries, stale >4s dropped)
                  → dispatcher (sanitise, per-bot 8s cooldown, ≤3 lines/10s,
                                delay + typing-time pacing)
                       → RCON ai_say <userid> <text>
```

Design invariants:

- **"Say nothing" is a valid, common LLM output** (`{"messages": []}`).
- **A missed quip beats a late one** — no retries, hard staleness drop.
- **The LLM never sees information a player couldn't know** (enforced by
  constructing the prompt only from log-visible state).
- **LLM text is untrusted** on its way into a server command — quotes,
  semicolons, newlines, `%` and the literal `rcon` are stripped.

## Personas

Bots come from `tf_bot_quota fill` with random TF2 names; the director assigns
one of 11 personas (`director/ailobby_director/personas.yaml`) to each bot name
in first-seen order, stable per session. Persona = personality + style +
chattiness weight + example lines; attribution in-game uses the bot's actual
name. (Fixed names via `tf_bot_add <name>` was considered but rejected for the
PoC: quota-fill keeps the server playable when the director is down.)

## Cost

~60–90 event calls + periodic calls per 30-min match, small prompts, Haiku
pricing ($1/$5 per MTok): **≈ $0.10–0.30 per match**. The director logs
per-call usage; totals print at shutdown.

## Known unknowns (verify at M2/M3)

- Whether stock srcds emits `chargedeployed` (logs.tf gets some events from the
  supstats2 plugin). Fallback: our plugin already logs uber % continuously, and
  `medic_death` is stock.
- `FakeClientCommandEx` "say" rendering for bots — fallback is emitting a
  `SayText2` usermessage from the same plugin.
- Exact log-line variants — parser treats unknowns as `RawEvent` (never fatal);
  replace synthetic test fixtures with real captures.
