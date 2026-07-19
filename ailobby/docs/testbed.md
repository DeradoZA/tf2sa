# 6v6 Testbed & Experimentation Workflow

How to launch a 6v6 game, watch the bots play, and iterate on behaviour fast.

## Config layers (compose in server/cfg/)

| File | Purpose | Where |
|---|---|---|
| `server.cfg` | logging → director, base settings | always exec'd on boot |
| `ailobby.cfg` | casual koth default (11-bot fill) | default game mode |
| `sixes.cfg` | 6v6 roster (2/2/1/1 per team) + comp settings | exec when testing 6s |
| `dev.cfg` | `sv_cheats 1`, `sv_lan 1`, dev toolkit | experimentation only — never on VPS |

Switch mode over RCON without restarting the container:
```
rcon exec dev
rcon exec sixes
rcon changelevel koth_viaduct
```

## The nav-mesh gotcha (read this before picking a map)

Stock TFBot pathing needs a `.nav` mesh. **koth_viaduct and the old-school
comp maps (cp_badlands, cp_granary) ship or handle nav well — start there.**
Modern 6s maps (cp_process, cp_gullywash, cp_snakewater) do **not** ship a nav
mesh; bots won't move until you generate one:

```
rcon sv_cheats 1
rcon nav_generate        # takes a few minutes; writes tf/maps/<map>.nav into the volume
```

Generated navs are rough — fine for building/testing the control layer, but
they don't encode 5CP rollouts or jump routes. Don't let bad nav be the thing
you're debugging while building behaviour; keep the confounds separate by
developing on a known-good-nav map first.

## Baseline: watch bots play 6v6

```
# 1. start the rig (see runbook-local.md)
# 2. load the 6s roster + map
rcon changelevel koth_viaduct
rcon exec sixes
# 3. connect from TF2 (Windows), join spectator to watch:
#    jointeam spectator   (in the TF2 console once connected)
```

Bots will fight over the point immediately — that's "them going off" with zero
extra work. Everything we build (discipline layer, movement, LLM IGL) layers on
top of this baseline.

## Fast iteration loop

```
rcon exec dev            # cheats + LAN
rcon bot_stop 1          # freeze the field to set up a scenario
rcon tf_bot_kill all     # clear
rcon tf_bot_add 1 soldier red hard   # spawn exactly what you want to test
rcon bot_stop 0          # let it run
rcon record trial_01     # capture a demo to review the movement/behaviour
```

Observability, in order of usefulness:
1. **Director log** — every kill/uber/capture event live (`docker compose -p ailobby logs -f director`)
2. **Spectator cam** in-game (`spec_mode`, `spec_next`) — watch a specific bot
3. **Demos** (`record`/`stop`) — review movement frame-by-frame later
4. **Bot debug overlays** — `tf_bot_debug 1`, `nb_debug`

## Measuring the goal (kills / DPM cap)

The director already parses every kill event, so the same pipeline that voices
the bots can **measure whether a human is being capped** — a stats accumulator
that reports per-human kills and DPM at round/match end (this mirrors the parent
TF2SA project, which parses the same logs.tf-format event stream). That's the
feedback loop for the <30-kill / 8000-DPM goal: play a session, read the number,
tune the discipline layer, repeat. (Accumulator to be added with the discipline
workstream — see the plan.)
