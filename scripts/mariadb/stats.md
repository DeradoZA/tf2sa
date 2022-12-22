# stats exploration
brief notes on data we pull from logstf - based on their http response  
shows what we pull from where, and why

## root level -> Games  
root-level attributes used to identify a game entry and info on that log
 | JSON Field  | Included(DB Field)? | Notes | 
 | --- | --- | --- |
 | N/A | **GameID** | REQUIRED |
 | N/A | **IsValidStats** | REQUIRED |
 | N/A | **InvalidStatsReason** | REQUIRED |
 | `version` | **Version** | NULLABLE |
 | `teams.Red.score` | **RedScore** | REQUIRED |
 | `teams.Blue.score` | **BlueScore** | REQUIRED |
 | `length` | **Duration** | NULLABLE |
 | `players` | No | see PlayerStats |
 | `names` | No | see Players |
 | `rounds` | No | very low value - future enhancement |
 | `healspread` | No | inferrable via PlayerStats |
 | `classkills` | No | low value - future enhancement |
 | `classdeaths` | No | low value - future enhancement |
 | `classkillassists` | No | low value - future enhancement |
 | `chat` | No | lol no |
 | `info.map` | **Map** | NULLABLE |
 | `info.supplemental` | **IsSupplemental** | NULLABLE - not sure what this means |
 | `info.total_length` | No | see Duration Field here |
 | `info.hasRealDamage` | **HasRealDamage**  | NULLABLE - weird damage values looks like we don't use |
 | `info.hasWeaponDamage` | **HasWeaponDamage**  | REQUIRED - useful |
 | `info.hasAccuracy` | **HasAccuracy**  | REQUIRED - we'd like to quantify this |
 | `info.hasHP` | **HasHP** | REQUIRED |
 | `info.hasHP_real` | **HasHPReal** | NULLABLE - weird real dmg |
 | `info.hasHS` | **HasHeadshots**  | REQUIRED |
 | `info.hasHS_hit` | **HasHeadshotsHit**  | REQUIRED |
 | `info.hasBS` | **HasBackstabs**  | REQUIRED |
 | `info.hasCP` | **HasCapturePointsCaptured**  | REQUIRED |
 | `info.hasSB` | **HasSentriesBuilt**  | REQUIRED |
 | `info.hasDT` | **HasDamageTaken**  | REQUIRED |
 | `info.hasAS` | **HasAirshots** | REQUIRED duh |
 | `info.hasHR` | **HasHealsReceived**  | REQUIRED |
 | `info.hasIntel` | **HasIntelCaptures**  | REQUIRED |
 | `info.AD_scoring` | **HasADScoring**  | NULLABLE - dont know what this is |
 | `info.notifications` | **Notifications**  | NULLABLE |
 | `info.title` | **Title** | NULLABLE - non-essential |
 | `info.date` | **Date**  | REQUIRED - useful to drive progression stats |
 | `info.uploader.id` | **UploaderID**  | NULLABLE |
 | `info.uploader.name` | **UploaderName**  | NULLABLE |
 | `info.uploader.info` | **UploaderInfo**  | NULLABLE |
 | `killstreaks` | No |  |
 | `success` | **Success**  | NULLABLE |

## names[steamid].* -> Players  
 | JSON Field  | Included(DB Field)? | Notes | 
 | --- | --- | --- |
 | N/A | **SteamID** | REQUIRED |
 | N/A | **PlayerName** | NULLABLE |

## players[steamid].* -> PlayerStats
 | JSON Field  | Included(DB Field)? | Notes | 
 | --- | --- | --- |
 | N/A | **PlayerStatsID** | REQUIRED |
 | N/A | **GameID** | REQUIRED |
 | N/A | **SteamID** | REQUIRED |
 | `team` | **TeamID** | REQUIRED |
 | `class_stats` | No | see ClassStats |
 | `kills` | No | see ClassStats - inferrable per class |
 | `deaths` | No | see ClassStats - inferrable per class |
 | `suicides` | No |  |
 | `kapd` | No | see ClassStats - inferrable per class |
 | `kpd` | No | see ClassStats - inferrable per class |
 | `dmg` | No | see ClassStats - inferrable per class |
 | `dmg_real` | No | see ClassStats - inferrable per class |
 | `dt` | **DamageTaken** | Nullable  - HasDamageTaken must be true |
 | `dt_real` | No | |
 | `hr` | **HealsReceived** | Nullable - HasHealsReceived must be true |
 | `lks` | **LongestKillstreak** | REQUIRED |
 | `as` | **Airshots** | REQUIRED |
 | `dapd` | No | |
 | `dapm` | No | see ClassStats - inferrable per class|
 | `ubers` | **Ubers** | Nullable - should only be counted when playing medic |
 | `ubertypes` | No | low value - future enhancement |
 | `drops` | **Drops** | Nullable - should only be counted when playing medic |
 | `medkits` | **Medkits** | REQUIRED |
 | `medkits_hp` | **MedkitsHP** | REQUIRED |
 | `backstabs` | **Backstabs** | Nullable - HasBackstabs must be true and played spy |
 | `headshots` | **Headshots** | Nullable - HasHeadshots must be true and played sniper |
 | `headshots_hit` | **HeadshotsHit** | Nullable - HasHeadshotsHit must be true and played sniper |
 | `sentries` | **SentriesBuilt** | Nullable - HasSentriesBuilt must be true and played engie |
 | `heal` | **Heals** | Nullable - should only be counted when playing medic |
 | `cpc` | **CapturePointsCaptured** | Nullable - HasCapturePointsCaptured must be true |
 | `ic` | **IntelCaptures** | Nullable - HasIntelCaptures must be true and gamemmode involves intel |

## players[steamid].class_stats.* -> ClassStats
 | JSON Field  | Included(DB Field)? | Notes | 
 | --- | --- | --- |
 | N/A | **ClassStatsID** | REQUIRED |
 | N/A | **PlayerStatsID** | REQUIRED |
 | `type` | **ClassID** | REQUIRED |
 | `kills` | **Kills** | REQUIRED |
 | `assists` | **Assists** | REQUIRED |
 | `deaths` | **Deaths** | REQUIRED |
 | `dmg` | No |  |
 | `weapon` | No | See WeaponStats |
 | `total_time` | **Playtime** | REQUIRED |