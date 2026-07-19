# Runbook — VPS rig (future)

Target: any Linux VPS with docker + compose, ideally SA-hosted for latency.

## Differences from local

1. `.env` additionally needs:
   - `SRCDS_TOKEN` — Game Server Login Token (AppID 440) from
     https://steamcommunity.com/dev/managegameservers — required for the
     server to be publicly reachable/listed.
   - `LOG_SECRET` — **required**. Spoofed UDP log packets are trivial without
     it. Also uncomment/set `sv_logsecret <value>` in `server/cfg/server.cfg`
     (cfg files can't read env vars — keep the two in sync).
2. Start with the VPS override:

   ```bash
   docker compose -p ailobby -f docker-compose.yml -f docker-compose.vps.yml up -d
   ```

## Firewall (host networking exposes everything — lock it down)

| Port | Action |
|---|---|
| 27015/udp | Open (game traffic) |
| 27015/tcp | **Restrict to admin IPs** (RCON — binds publicly under host net) |
| 27100/udp | **Block from outside** (director log listener; traffic is loopback) |
| everything else | Default deny |

Example (ufw):

```bash
ufw allow 27015/udp
ufw allow from <admin-ip> to any port 27015 proto tcp
ufw deny 27100/udp
```

## Operational notes

- The `cm2network/tf2` image updates the game on container restart. TF2 updates
  occasionally break SourceMod until AlliedModders rebuilds — during those
  windows, avoid restarting the tf2 container.
- Director is stateless; safe to rebuild/redeploy anytime.
- Watch spend: the director logs per-call token usage and prints totals on
  shutdown (~$0.10–0.30 per 30-min match at Haiku pricing).
