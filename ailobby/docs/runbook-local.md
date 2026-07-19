# Runbook — Local rig (Windows + WSL2)

Verified assumptions for this machine: **native docker-ce inside WSL2** (not
Docker Desktop — good, Desktop's host-network UDP support is flaky) and WSL
networking currently in **NAT mode** (`wslinfo --networking-mode` → `nat`).

## 1. One-time setup

```bash
cd ailobby
cp .env.example .env      # set RCON_PASSWORD (anything strong) + ANTHROPIC_API_KEY
./plugin/build.sh         # compiles plugin/ailobby.sp → server overlay
```

## 2. Networking: how the Windows TF2 client reaches the server

Game traffic is **UDP 27015**. Two options:

### Option A — NAT mode (current, zero config)

Windows → WSL direct-IP traffic routes fine under NAT; only *localhost*
forwarding is TCP-only. So connect to the WSL IP:

```bash
hostname -I      # e.g. 172.20.41.3
```

In TF2 (Windows), open console (`~`) and:

```
connect 172.20.41.3:27015
```

The WSL IP can change across `wsl --shutdown`; re-check `hostname -I`.

### Option B — Mirrored networking (nicer, Windows 11 22H2+)

In Windows, edit `%UserProfile%\.wslconfig`:

```ini
[wsl2]
networkingMode=mirrored
```

Then from PowerShell: `wsl --shutdown`, reopen WSL. Now `connect 127.0.0.1:27015`
works (mirrored mode forwards UDP on localhost). If the connection times out,
allow inbound on the Hyper-V firewall (admin PowerShell):

```powershell
Set-NetFirewallHyperVVMSetting -Name '{40E0AC32-46A5-438A-A0B2-2B479E8F2E90}' -DefaultInboundAction Allow
```

## 3. Start the rig

```bash
docker compose -p ailobby -f docker-compose.yml -f docker-compose.local.yml up -d
docker compose -p ailobby logs -f tf2      # watch the SteamCMD download (~20 GB, first run only)
```

The server is up when logs show the map load and `tf_bot` chatter. Verify:

```bash
docker compose -p ailobby logs -f director   # should show "listening for srcds log packets"
```

## 4. Verify milestones

- **M1**: connect from TF2; 11 bots fighting on viaduct. RCON check from WSL:
  the director's logs show chat lines; or in the TF2 console
  `rcon_password <pw>; rcon status`.
- **M2**: play for a minute — director logs typed events for your kills/chat.
  Capture real lines for the test fixtures:
  `docker compose -p ailobby logs director --no-log-prefix | grep 'raw:'`
- **M3**: kill the enemy medic → a bot should comment within ~2s, attributed
  with its name and team colour. Manual plumbing test from WSL:
  first get a bot's userid (`rcon status` in the TF2 console), then
  `rcon ai_say <userid> test line` (via the in-game console: `rcon ai_say ...`).

## 5. Iterate on the feel (M4)

Prompt/persona changes need no rebuild of the game server:

```bash
docker compose -p ailobby up -d --build director
```

Tunables are env vars (see `director/ailobby_director/config.py`); model swaps
via `AILOBBY_MODEL` in `.env` (e.g. `claude-sonnet-4-6` for a quality A/B).

## Troubleshooting

| Symptom | Check |
|---|---|
| Client can't connect | NAT: using WSL IP, not localhost? `sudo ss -ulpn \| grep 27015` shows srcds bound? Windows firewall? |
| No events in director | `logaddress_add` present in `rcon status`-visible cvars? Both containers on host network? |
| Bots don't speak | `rcon sm plugins list` shows "AI Lobby Bridge"? Director logs show LLM calls but delivery failures? RCON password matches? |
| SourceMod missing | First boot installs the game *then* the image layers SM — check `tf/addons/sourcemod` exists in the volume; restart the container once after the initial download |
