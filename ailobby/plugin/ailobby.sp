/**
 * AI Lobby bridge plugin.
 *
 * Deliberately tiny — the only two things RCON + remote logging can't do:
 *   1. `ai_say <userid> <message>` server command: makes a specific bot speak
 *      as normal, attributed player chat (name + team colour).
 *   2. A repeating timer that logs medic uber % into the game log, which flows
 *      out over the existing logaddress_add UDP stream to the AI Director.
 *
 * Everything else (event ingestion, LLM calls, rate limiting) lives in the
 * Python director. No HTTP/socket extensions, no threading in Pawn.
 */

#include <sourcemod>
#include <sdktools>
#include <tf2_stocks>

#pragma semicolon 1
#pragma newdecls required

public Plugin myinfo =
{
	name = "AI Lobby Bridge",
	author = "TF2SA",
	description = "Attributed bot chat + uber telemetry for the AI Director",
	version = "0.1.0",
	url = "https://tf2sa.co.za"
};

// Per-client damage accumulated since the last flush, keyed by client index.
int g_Damage[MAXPLAYERS + 1];

public void OnPluginStart()
{
	RegServerCmd("ai_say", Cmd_AiSay, "ai_say <userid> <message> - make a bot speak in chat");
	HookEvent("player_hurt", Event_PlayerHurt);
	CreateTimer(3.0, Timer_UberStatus, _, TIMER_REPEAT);
	// Flush accumulated damage into the log stream every 5s (feeds DPM in the
	// director). Batched rather than per-hit to keep log volume sane.
	CreateTimer(5.0, Timer_FlushDamage, _, TIMER_REPEAT);
}

void Event_PlayerHurt(Event event, const char[] name, bool dontBroadcast)
{
	int attacker = GetClientOfUserId(event.GetInt("attacker"));
	int victim = GetClientOfUserId(event.GetInt("userid"));
	// Count only enemy-directed damage: ignore world/suicide and self-damage
	// (rocket/sticky jumps shouldn't inflate DPM).
	if (attacker <= 0 || attacker > MaxClients || attacker == victim)
	{
		return;
	}
	if (!IsClientInGame(attacker))
	{
		return;
	}
	int dmg = event.GetInt("damageamount");
	if (dmg > 0)
	{
		g_Damage[attacker] += dmg;
	}
}

Action Timer_FlushDamage(Handle timer)
{
	for (int client = 1; client <= MaxClients; client++)
	{
		if (g_Damage[client] <= 0)
		{
			continue;
		}
		if (!IsClientInGame(client))
		{
			g_Damage[client] = 0;
			continue;
		}
		char auth[32];
		if (!GetClientAuthId(client, AuthId_Steam3, auth, sizeof(auth)))
		{
			strcopy(auth, sizeof(auth), "BOT");
		}
		char team[16];
		GetTeamName(GetClientTeam(client), team, sizeof(team));
		// Same shape supstats2 uses, which the director's parser already handles.
		LogToGame("\"%N<%d><%s><%s>\" triggered \"damage\" (damage \"%d\")",
			client, GetClientUserId(client), auth, team, g_Damage[client]);
		g_Damage[client] = 0;
	}
	return Plugin_Continue;
}

Action Cmd_AiSay(int args)
{
	if (args < 2)
	{
		PrintToServer("[ailobby] usage: ai_say <userid> <message>");
		return Plugin_Handled;
	}

	char buf[16];
	GetCmdArg(1, buf, sizeof(buf));
	int client = GetClientOfUserId(StringToInt(buf));

	if (client <= 0 || !IsClientInGame(client) || !IsFakeClient(client))
	{
		PrintToServer("[ailobby] ai_say: no in-game bot with that userid");
		return Plugin_Handled;
	}

	// Join args 2..N so unquoted messages work; director sanitises content.
	char msg[192], part[128];
	for (int i = 2; i <= args; i++)
	{
		GetCmdArg(i, part, sizeof(part));
		if (i > 2)
		{
			StrCat(msg, sizeof(msg), " ");
		}
		StrCat(msg, sizeof(msg), part);
	}

	// Renders as regular player chat (attributed, team-coloured) and also
	// appears in the log stream — a free delivery confirmation for the director.
	FakeClientCommandEx(client, "say %s", msg);
	return Plugin_Handled;
}

Action Timer_UberStatus(Handle timer)
{
	for (int client = 1; client <= MaxClients; client++)
	{
		if (!IsClientInGame(client) || !IsPlayerAlive(client))
		{
			continue;
		}
		if (TF2_GetPlayerClass(client) != TFClass_Medic)
		{
			continue;
		}

		int medigun = GetPlayerWeaponSlot(client, 1);
		if (medigun == -1 || !HasEntProp(medigun, Prop_Send, "m_flChargeLevel"))
		{
			continue;
		}
		float charge = GetEntPropFloat(medigun, Prop_Send, "m_flChargeLevel");

		char auth[32];
		if (!GetClientAuthId(client, AuthId_Steam3, auth, sizeof(auth)))
		{
			strcopy(auth, sizeof(auth), "BOT");
		}
		char team[16];
		GetTeamName(GetClientTeam(client), team, sizeof(team));

		// Standard HL-log "triggered" shape so the director's parser handles it
		// like any other event line.
		LogToGame("\"%N<%d><%s><%s>\" triggered \"ailobby_uber\" (charge \"%.2f\")",
			client, GetClientUserId(client), auth, team, charge);
	}
	return Plugin_Continue;
}
