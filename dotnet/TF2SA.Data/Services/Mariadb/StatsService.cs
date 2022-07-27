using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TF2SA.Data.Constants;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Models;
using TF2SA.Data.Repositories.Base;
using TF2SA.Data.Services.Base;

namespace TF2SA.Data.Services.Mariadb
{
    public class StatsService : IStatsService<ulong>
    {

        private readonly IPlayersRepository<Player, ulong> playerRepository;
        private readonly IPlayerStatsRepository<PlayerStat, uint> playerStatRepository;
        private readonly IClassStatsRepository<ClassStat, uint> classStatsRepository;
        private readonly IGamesRepository<Game, uint> gamesRepository;
        private readonly ILogger<StatsService> logger;
        public StatsService(
            IPlayersRepository<Player, ulong> playerRepository,
            IPlayerStatsRepository<PlayerStat, uint> playerStatRepository,
            IClassStatsRepository<ClassStat, uint> classStatsRepository,
            ILogger<StatsService> logger,
            IGamesRepository<Game, uint> gamesRepository)
        {
            this.playerRepository = playerRepository;
            this.playerStatRepository = playerStatRepository;
            this.classStatsRepository = classStatsRepository;
            this.logger = logger;
            this.gamesRepository = gamesRepository;
        }

        public IQueryable<JoinedStats> PlayerStatsJoinQueryable()
        {
            var players = playerRepository.GetAllQueryable();
            var playerStats = playerStatRepository.GetAllQueryable();
            var classStats = classStatsRepository.GetAllQueryable();
            var gameStats = gamesRepository.GetAllQueryable();

            var InnerJoinQuery = 
                from player in players
                join playerStat in playerStats on player.SteamId equals playerStat.SteamId
                join classStat in classStats on playerStat.PlayerStatsId equals classStat.PlayerStatsId
                join gameStat in gameStats on playerStat.GameId equals gameStat.GameId
                select new JoinedStats
                    (
                        player.SteamId, 
                        player.PlayerName, 
                        playerStat.PlayerStatsId,
                        playerStat.GameId, 
                        playerStat.TeamId, 
                        playerStat.DamageTaken,
                        playerStat.HealsReceived, 
                        playerStat.MedkitsHp, 
                        playerStat.Airshots,
                        playerStat.Headshots, 
                        playerStat.Backstabs, 
                        playerStat.Drops,
                        playerStat.Heals, 
                        playerStat.Ubers, 
                        classStat.ClassStatsId,
                        classStat.ClassId, 
                        classStat.Playtime,
                        classStat.Kills,
                        classStat.Assists, 
                        classStat.Deaths, 
                        classStat.Damage,
                        gameStat.Date,
                        gameStat.Duration,
                        gameStat.Map,
                        gameStat.BluScore,
                        gameStat.RedScore
                    );

            return InnerJoinQuery;
        }

        public List<JoinedStats> PlayerStatsJoinList()
        {
            return PlayerStatsJoinQueryable().ToList();
        }

        public List<PlayerPerformanceStats> AllTimeStats()
        {
            var playerNumberOfGames = PlayerGameCounter(StatsCollectionConstants.ALLTIME_THRESHOLD);
            var playerGeneralStats = MainStatsCollector(StatsCollectionConstants.ALLTIME_THRESHOLD);
            var playerAirshots = AirshotStatsCollector(StatsCollectionConstants.ALLTIME_THRESHOLD);
            var playerHeadshots = HeadshotStatsCollector(StatsCollectionConstants.ALLTIME_THRESHOLD);

            var completeLeaderboard = (
                from png in playerNumberOfGames
                join pgs in playerGeneralStats on png.SteamId equals pgs.SteamId
                join pa in playerAirshots on png.SteamId equals pa.SteamId
                join ph in playerHeadshots on png.SteamId equals ph.SteamId
                where png.NumberOfGames > StatsCollectionConstants.PLAYER_NUMBEROFGAMES_THRESHOLD
                orderby pgs.DPM descending
                select new PlayerPerformanceStats(
                    png.SteamId,
                    png.SteamName,
                    png.NumberOfGames,
                    pgs.DPM,
                    pgs.Kills,
                    pgs.Assists,
                    pgs.Deaths,
                    pa.Airshots,
                    ph.Headshots
                ));

            return completeLeaderboard.ToList();
        }


        private List<PlayerNumGames> PlayerGameCounter(int timeFrame, int classID = 0, ulong steamID = 0)
        {
            var joinedPlayerStats = PlayerStatsJoinList();

            var allGames = joinedPlayerStats.Where(game => game.Date > DateTimeOffset.Now.ToUnixTimeSeconds() - timeFrame);

            if (classID != 0)
            {
                allGames = allGames.Where(game => game.ClassId == classID);
            }

            if (steamID != 0)
            {
                allGames = allGames.Where(game => game.SteamId == steamID);
            }

            var groupedPlayerGames = allGames.GroupBy(player => player.SteamId);

            var groupedPlayerDistinctGames =
                groupedPlayerGames.Select(game => 
                    new PlayerNumGames(
                        game.Key,
                        game.FirstOrDefault().PlayerName,
                        game.Select(g => g.PlayerStatsId).Distinct().Count()
                ));

            return groupedPlayerDistinctGames.ToList();                                                           
        }

        public List<AverageMainStats> MainStatsCollector(int timeFrame, int classID = 0, ulong steamID = 0, bool avg = true)
        {
            var joinedPlayerStats = PlayerStatsJoinList();
            var allGames = joinedPlayerStats.Where(game =>
                game.Date > DateTimeOffset.Now.ToUnixTimeSeconds() - timeFrame &&
                game.Playtime > StatsCollectionConstants.PLAYTIME_THRESHOLD
            );

            if (classID == 0)
            {
                allGames = allGames.Where(game => game.ClassId != 7);
            }
            else
            {
                allGames = allGames.Where(game => game.ClassId == classID);
            }

            if (steamID != 0)
            {
                allGames = allGames.Where(game => game.SteamId == steamID);
            }

            var groupedPlayerGames = allGames.GroupBy(player => player.SteamId);

            var averagedStats = groupedPlayerGames.Select(
                player => new AverageMainStats(
                    player.Key,
                    (avg ? player.Average(d => d.Damage / (d.Playtime / 60)) : player.Max(d => d.Damage / (d.Playtime / 60))),
                    (avg ? player.Average(k => k.Kills) : player.Max(k => k.Kills)),
                    (avg ? player.Average(a => a.Assists) : player.Max(a => a.Assists)),
                    (avg ? player.Average(de => de.Deaths) : player.Max(de => de.Deaths))
            ));

            return averagedStats.ToList();
        }

        private List<AverageAirshots> AirshotStatsCollector(int timeFrame, int classID = 0, ulong steamID = 0, bool avg=true)
        {
            var joinedPlayerStats = PlayerStatsJoinList();

            var allGames = joinedPlayerStats.Where(game => game.Date > DateTimeOffset.Now.ToUnixTimeSeconds() - timeFrame);

            if (classID == 0)
            {
                allGames = allGames.Where(game => 
                    game.ClassId == StatsCollectionConstants.SOLDIER_CLASSID ||
                    game.ClassId == StatsCollectionConstants.DEMOMAN_CLASSID);
            }
            else
            {
                allGames = allGames.Where(game =>
                    game.ClassId == classID);
            }

            if (steamID != 0)
            {
                allGames = allGames.Where(game => game.SteamId == steamID);
            }

            var groupedPlayerGames = allGames.GroupBy(game => game.SteamId);

            var averagedAirshots = groupedPlayerGames.Select(player =>
                new AverageAirshots(
                    player.Key,
                    (avg ? player.Average(a => a.Airshots) : player.Max(a => a.Airshots))
                ));

            return averagedAirshots.ToList();
        }

        private List<AverageMedicStats> MedicStatsCollector(int timeFrame, ulong steamID = 0, bool avg = true)
        {
            var joinedPlayerStats = PlayerStatsJoinList();

            var allGames = joinedPlayerStats.Where(game =>
                game.Date > DateTimeOffset.Now.ToUnixTimeSeconds() - timeFrame &&
                game.ClassId == StatsCollectionConstants.MEDIC_CLASSID &&
                game.Playtime > StatsCollectionConstants.PLAYTIME_THRESHOLD);

            if (steamID != 0)
            {
                allGames = allGames.Where(game => game.SteamId == steamID);
            }

            var groupedPlayerGames = allGames.GroupBy(game => game.SteamId);    

            var averagedMedicStats = groupedPlayerGames.Select(player =>
            new AverageMedicStats(
                player.Key,
                (avg ? player.Average(d => d.Drops) : player.Max(d => d.Drops)),
                (avg ? player.Average(u => u.Ubers) : player.Max(u => u.Ubers)),
                (avg ? player.Average(h => h.Heals / (h.Playtime / 60)) : player.Max(h => h.Heals / (h.Playtime / 60)))
            ));

            return averagedMedicStats.ToList();
        }

        private List<AverageHeadshots> HeadshotStatsCollector(int timeFrame, ulong steamID = 0, bool avg = true)
        {
            var joinedPlayerStats = PlayerStatsJoinList();

            var allGames = joinedPlayerStats.Where(game => 
                game.Date > DateTimeOffset.Now.ToUnixTimeSeconds() - timeFrame &&
                game.ClassId == StatsCollectionConstants.SNIPER_CLASSID
            );

            if (steamID != 0)
            {
                allGames = allGames.Where(game => game.SteamId == steamID);
            }

            var groupedPlayerGames = allGames.GroupBy(game => game.SteamId);

            var averagedHeadshots = groupedPlayerGames.Select(player =>
                new AverageHeadshots(
                    player.Key,
                    (avg ? player.Average(h => h.Headshots) : player.Max(h => h.Headshots))
                ));

            return averagedHeadshots.ToList();
        }
        public List<PlayerPerformanceStats> RecentStats()
        {
            
            var playerNumberOfGames = PlayerGameCounter(StatsCollectionConstants.RECENTGAMES_THRESHOLD);
            var playerGeneralStats = MainStatsCollector(StatsCollectionConstants.RECENTGAMES_THRESHOLD);
            var playerAirshots = AirshotStatsCollector(StatsCollectionConstants.RECENTGAMES_THRESHOLD);
            var playerHeadshots = HeadshotStatsCollector(StatsCollectionConstants.RECENTGAMES_THRESHOLD);

            var completeLeaderboard = (
                from png in playerNumberOfGames
                join pgs in playerGeneralStats on png.SteamId equals pgs.SteamId
                join pa in playerAirshots on png.SteamId equals pa.SteamId
                join ph in playerHeadshots on png.SteamId equals ph.SteamId
                where png.NumberOfGames > StatsCollectionConstants.PLAYER_NUMBEROFGAMES_THRESHOLD
                orderby pgs.DPM descending
                select new PlayerPerformanceStats(
                    png.SteamId,
                    png.SteamName,
                    png.NumberOfGames,
                    pgs.DPM,
                    pgs.Kills,
                    pgs.Assists,
                    pgs.Deaths,
                    pa.Airshots,
                    ph.Headshots
                )
            );
            
            return completeLeaderboard.ToList();
        }

        public List<ScoutPerformanceStats> ScoutStatsAllTime()
        {
            var scoutGames = PlayerGameCounter(StatsCollectionConstants.ALLTIME_THRESHOLD, StatsCollectionConstants.SCOUT_CLASSID);
            var scoutPerformance = MainStatsCollector(StatsCollectionConstants.ALLTIME_THRESHOLD, StatsCollectionConstants.SCOUT_CLASSID);

            var completeLeaderboard = (
                from sg in scoutGames
                join sp in scoutPerformance on sg.SteamId equals sp.SteamId
                where sg.NumberOfGames > StatsCollectionConstants.PLAYER_NUMBEROFGAMES_THRESHOLD
                orderby sp.DPM descending
                select new ScoutPerformanceStats
                (
                    sg.SteamId,
                    sg.SteamName,
                    sg.NumberOfGames,
                    sp.DPM,
                    sp.Assists,
                    sp.Assists,
                    sp.Deaths
                )
            );

            return completeLeaderboard.ToList();
        }

        public List<ScoutPerformanceStats> ScoutStatsRecent()
        {
            var scoutGames = PlayerGameCounter(StatsCollectionConstants.RECENTGAMES_THRESHOLD, StatsCollectionConstants.SCOUT_CLASSID);
            var scoutPerformance = MainStatsCollector(StatsCollectionConstants.RECENTGAMES_THRESHOLD, StatsCollectionConstants.SCOUT_CLASSID);

            var completeLeaderboard = (
                from sg in scoutGames
                join sp in scoutPerformance on sg.SteamId equals sp.SteamId
                where sg.NumberOfGames > StatsCollectionConstants.PLAYER_NUMBEROFGAMES_THRESHOLD
                orderby sp.DPM descending
                select new ScoutPerformanceStats
                (
                    sg.SteamId,
                    sg.SteamName,
                    sg.NumberOfGames,
                    sp.DPM,
                    sp.Kills,
                    sp.Assists,
                    sp.Deaths
                )
            );

            return completeLeaderboard.ToList();
        }

        public List<ExplosiveClassStats> SoldierStatsAllTime(ulong steamid = 0)
        {
            var soldierGames = PlayerGameCounter(StatsCollectionConstants.ALLTIME_THRESHOLD, StatsCollectionConstants.SOLDIER_CLASSID);
            var SoldierPerformance = MainStatsCollector(StatsCollectionConstants.ALLTIME_THRESHOLD, StatsCollectionConstants.SOLDIER_CLASSID);
            var soldierAirshots = AirshotStatsCollector(StatsCollectionConstants.ALLTIME_THRESHOLD,StatsCollectionConstants.SOLDIER_CLASSID);

            var completeLeaderboard = (
                from sg in soldierGames
                join sp in SoldierPerformance on sg.SteamId equals sp.SteamId
                join sa in soldierAirshots on sg.SteamId equals sa.SteamId
                where sg.NumberOfGames > StatsCollectionConstants.PLAYER_NUMBEROFGAMES_THRESHOLD
                orderby sp.DPM descending
                select new ExplosiveClassStats
                (
                    sg.SteamId,
                    sg.SteamName,
                    sg.NumberOfGames,
                    sp.DPM,
                    sp.Kills,
                    sp.Assists,
                    sp.Deaths,
                    sa.Airshots
                )
            );

            if (steamid != 0)
            {
                completeLeaderboard = completeLeaderboard.Where(player => player.SteamId == steamid);
            }

            return completeLeaderboard.ToList();
        }

        public List<ExplosiveClassStats> SoldierStatsRecent()
        {
            var soldierGames = PlayerGameCounter(StatsCollectionConstants.RECENTGAMES_THRESHOLD, StatsCollectionConstants.SOLDIER_CLASSID);
            var SoldierPerformance = MainStatsCollector(StatsCollectionConstants.RECENTGAMES_THRESHOLD, StatsCollectionConstants.SOLDIER_CLASSID);
            var soldierAirshots = AirshotStatsCollector(StatsCollectionConstants.RECENTGAMES_THRESHOLD,StatsCollectionConstants.SOLDIER_CLASSID);

            var completeLeaderboard = (
                from sg in soldierGames
                join sp in SoldierPerformance on sg.SteamId equals sp.SteamId
                join sa in soldierAirshots on sg.SteamId equals sa.SteamId
                where sg.NumberOfGames > StatsCollectionConstants.PLAYER_NUMBEROFGAMES_THRESHOLD
                orderby sp.DPM descending
                select new ExplosiveClassStats
                (
                    sg.SteamId,
                    sg.SteamName,
                    sg.NumberOfGames,
                    sp.DPM,
                    sp.Kills,
                    sp.Assists,
                    sp.Deaths,
                    sa.Airshots
                )
            );

            return completeLeaderboard.ToList();
        }

        public List<ExplosiveClassStats> DemomanStatsAllTime(ulong steamid = 0)
        {
            var demomanGames = PlayerGameCounter(StatsCollectionConstants.ALLTIME_THRESHOLD, StatsCollectionConstants.DEMOMAN_CLASSID);
            var demomanPerformance = MainStatsCollector(StatsCollectionConstants.ALLTIME_THRESHOLD, StatsCollectionConstants.DEMOMAN_CLASSID);
            var demomanAirshots = AirshotStatsCollector(StatsCollectionConstants.ALLTIME_THRESHOLD,StatsCollectionConstants.DEMOMAN_CLASSID);

            var completeLeaderboard = (
                from dg in demomanGames
                join dp in demomanPerformance on dg.SteamId equals dp.SteamId
                join da in demomanAirshots on dg.SteamId equals da.SteamId
                where dg.NumberOfGames > StatsCollectionConstants.PLAYER_NUMBEROFGAMES_THRESHOLD
                orderby dp.DPM descending
                select new ExplosiveClassStats
                (
                    dg.SteamId,
                    dg.SteamName,
                    dg.NumberOfGames,
                    dp.DPM,
                    dp.Kills,
                    dp.Assists,
                    dp.Deaths,
                    da.Airshots
                )
            );

            if (steamid != 0)
            {
                completeLeaderboard = completeLeaderboard.Where(player => player.SteamId == steamid);
            }

            return completeLeaderboard.ToList();
        }

        public List<ExplosiveClassStats> DemomanStatsRecent()
        {
            var demomanGames = PlayerGameCounter(StatsCollectionConstants.RECENTGAMES_THRESHOLD, StatsCollectionConstants.DEMOMAN_CLASSID);
            var demomanPerformance = MainStatsCollector(StatsCollectionConstants.RECENTGAMES_THRESHOLD, StatsCollectionConstants.DEMOMAN_CLASSID);
            var demomanAirshots = AirshotStatsCollector(StatsCollectionConstants.RECENTGAMES_THRESHOLD,StatsCollectionConstants.DEMOMAN_CLASSID);

            var completeLeaderboard = (
                from dg in demomanGames
                join dp in demomanPerformance on dg.SteamId equals dp.SteamId
                join da in demomanAirshots on dg.SteamId equals da.SteamId
                where dg.NumberOfGames > StatsCollectionConstants.PLAYER_NUMBEROFGAMES_THRESHOLD
                orderby dp.DPM descending
                select new ExplosiveClassStats
                (
                    dg.SteamId,
                    dg.SteamName,
                    dg.NumberOfGames,
                    dp.DPM,
                    dp.Kills,
                    dp.Assists,
                    dp.Deaths,
                    da.Airshots
                )
            );

            return completeLeaderboard.ToList();
        }

        public List<MedicPerformanceStats> MedicStatsAllTime(ulong steamid = 0)
        {
            var medicGames = PlayerGameCounter(StatsCollectionConstants.ALLTIME_THRESHOLD, StatsCollectionConstants.MEDIC_CLASSID);
            var medicCombatStats = MainStatsCollector(StatsCollectionConstants.ALLTIME_THRESHOLD, StatsCollectionConstants.MEDIC_CLASSID);
            var medicHealingStats = MedicStatsCollector(StatsCollectionConstants.ALLTIME_THRESHOLD);

            var completeLeaderboard = (
                from mg in medicGames
                join mcs in medicCombatStats on mg.SteamId equals mcs.SteamId
                join mhs in medicHealingStats on mg.SteamId equals mhs.SteamId
                where mg.NumberOfGames > StatsCollectionConstants.PLAYER_NUMBEROFGAMES_THRESHOLD
                orderby mhs.Heals descending
                select new MedicPerformanceStats(
                    mg.SteamId,
                    mg.SteamName,
                    mg.NumberOfGames,
                    mcs.DPM,
                    mcs.Kills,
                    mcs.Assists,
                    mcs.Deaths,
                    mhs.Drops,
                    mhs.Ubers,
                    mhs.Heals
                )
            );

            if (steamid != 0)
            {
                completeLeaderboard = completeLeaderboard.Where(player => player.SteamId == steamid);
            }

            return completeLeaderboard.ToList();
        }

        public List<MedicPerformanceStats> MedicStatsRecent()
        {
            var medicGames = PlayerGameCounter(StatsCollectionConstants.RECENTGAMES_THRESHOLD, StatsCollectionConstants.MEDIC_CLASSID);
            var medicCombatStats = MainStatsCollector(StatsCollectionConstants.RECENTGAMES_THRESHOLD, StatsCollectionConstants.MEDIC_CLASSID);
            var medicHealingStats = MedicStatsCollector(StatsCollectionConstants.RECENTGAMES_THRESHOLD);

            var completeLeaderboard = (
                from mg in medicGames
                join mcs in medicCombatStats on mg.SteamId equals mcs.SteamId
                join mhs in medicHealingStats on mg.SteamId equals mhs.SteamId
                where mg.NumberOfGames > StatsCollectionConstants.PLAYER_NUMBEROFGAMES_THRESHOLD
                orderby mhs.Heals descending
                select new MedicPerformanceStats(
                    mg.SteamId,
                    mg.SteamName,
                    mg.NumberOfGames,
                    mcs.DPM,
                    mcs.Kills,
                    mcs.Assists,
                    mcs.Deaths,
                    mhs.Drops,
                    mhs.Ubers,
                    mhs.Heals
                )
            );

            return completeLeaderboard.ToList();
        }

        public List<PlayerHighlights> PlayerHighlightCollector(ulong steamID)
        {
            var scoutHighlights = MainStatsCollector(StatsCollectionConstants.ALLTIME_THRESHOLD,StatsCollectionConstants.SCOUT_CLASSID, steamID, false);
            var soldierHighlights = MainStatsCollector(StatsCollectionConstants.ALLTIME_THRESHOLD, StatsCollectionConstants.SOLDIER_CLASSID, steamID, false);
            var maxSoldierAirshots = AirshotStatsCollector(StatsCollectionConstants.ALLTIME_THRESHOLD,StatsCollectionConstants.SOLDIER_CLASSID, steamID, false);
            var demomanHighlights = MainStatsCollector(StatsCollectionConstants.ALLTIME_THRESHOLD, StatsCollectionConstants.DEMOMAN_CLASSID, steamID, false);
            var maxDemomanAirshots = AirshotStatsCollector(StatsCollectionConstants.ALLTIME_THRESHOLD,StatsCollectionConstants.DEMOMAN_CLASSID, steamID, false);
            var medicHighlights = MedicStatsCollector(StatsCollectionConstants.ALLTIME_THRESHOLD, steamID, false);

            var playerHighlights = (
                from scoutGame in scoutHighlights
                join soldierGame in soldierHighlights on scoutGame.SteamId equals soldierGame.SteamId
                join sAirshots in maxSoldierAirshots on scoutGame.SteamId equals sAirshots.SteamId
                join demoGame in demomanHighlights on scoutGame.SteamId equals demoGame.SteamId
                join dAirshots in maxDemomanAirshots on scoutGame.SteamId equals dAirshots.SteamId
                join medicGame in medicHighlights on scoutGame.SteamId equals medicGame.SteamId
                select new PlayerHighlights
                (
                    steamID,
                    (uint?) scoutGame.DPM,
                    (byte?) scoutGame.Kills,
                    (uint?) soldierGame.DPM,
                    (byte?) soldierGame.Kills,
                    (byte?) sAirshots.Airshots,
                    (uint?) demoGame.DPM,
                    (byte?) demoGame.Kills,
                    (byte?) dAirshots.Airshots,
                    (byte?) medicGame.Drops,
                    (uint?) medicGame.Heals,
                    (byte?) medicGame.Ubers
                )
            );

            return playerHighlights.ToList();
        }

    }
    }

   