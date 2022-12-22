# stats exploration
brief notes on data we pull from logstf - based on their http response  
shows what we pull from where, and why

## root level -> Games  
root-level attributes used to identify a game entry and info on that log
 | JSON Field  | Included(DB Field)? | Notes | 
 | --- | --- | --- |
 | N/A | **GameID** | NULLABLE |
 | N/A | **IsValidStats** | REQUIRED |
 | N/A | **InvalidStatsReason** | REQUIRED |
 | `version` | **Version** | NULLABLE |
 | `teams` | No | detailed in Teams |
 | `length` | **Duration** | REQUIRED |
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
 | `info.hasHP` | **HasHP** | NULLABLE - this is probably less interesting |
 | `info.hasHP_real` | **HasHPReal** | NULLABLE - weird real dmg |
 | `info.hasHS` | **HasHeadshots**  | REQUIRED |
 | `info.hasHS_hit` | **HasHeadshotsHit**  | REQUIRED |
 | `info.hasBS` | **HasBackstabs**  | REQUIRED |
 | `info.hasCP` | **HasCapturePointsCaptured**  | NULLABLE - less interesting |
 | `info.hasSB` | **HasSentriesBuilt**  | NULLABLE - less interesting |
 | `info.hasDT` | **HasDamageTaken**  | REQUIRED |
 | `info.hasAS` | **HasAirshots** | REQUIRED duh |
 | `info.hasHR` | **HasHealsReceived**  | REQUIRED |
 | `info.hasIntel` | **HasIntelCaptures**  | NULLABLE - not interested in gamemode |
 | `info.AD_scoring` | **HasADScoring**  | NULLABLE - dont know what this is |
 | `info.notifications` | **Notifications**  | NULLABLE |
 | `info.title` | **Title** | NULLABLE - non-essential |
 | `info.date` | **Date**  | REQUIRED - useful to drive progression stats |
 | `info.uploader.id` | **UploaderID**  | NULLABLE |
 | `info.uploader.name` | **UploaderName**  | NULLABLE |
 | `info.uploader.info` | **UploaderInfo**  | NULLABLE |
 | `killstreaks` | No |  |
 | `success` | **Success**  | NULLABLE |

## teams.* -> Teams  
*TODO:* Draw up schema