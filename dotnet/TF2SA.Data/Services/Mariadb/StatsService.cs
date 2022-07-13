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
    public class StatsService : IStatsService
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
            var playerGeneralStats = AverageMainStatsCollector(StatsCollectionConstants.ALLTIME_THRESHOLD);
            var playerAirshots = AverageAirshotCollector(StatsCollectionConstants.ALLTIME_THRESHOLD);
            var playerHeadshots = AverageHeadshotCollector(StatsCollectionConstants.ALLTIME_THRESHOLD);

            var completeLeaderboard = (
                from png in playerNumberOfGames
                join pgs in playerGeneralStats on png.SteamId equals pgs.SteamId
                join pa in playerAirshots on png.SteamId equals pa.SteamId
                join ph in playerHeadshots on png.SteamId equals ph.SteamId
                where png.NumberOfGames > StatsCollectionConstants.PLAYER_NUMBEROFGAMES_THRESHOLD
                orderby pgs.AverageDPM descending
                select new PlayerPerformanceStats(
                    png.SteamId,
                    png.SteamName,
                    png.NumberOfGames,
                    pgs.AverageDPM,
                    pgs.AverageKills,
                    pgs.AverageAssists,
                    pgs.AverageDeaths,
                    pa.AirshotsAverage,
                    ph.HeadshotsAverage
                ));

            return completeLeaderboard.ToList();
        }


        private List<PlayerNumGames> PlayerGameCounter(int timeFrame, int classID = 0)
        {
            var joinedPlayerStats = PlayerStatsJoinList();

            var allGames = joinedPlayerStats.Where(game => game.Date > DateTimeOffset.Now.ToUnixTimeSeconds() - timeFrame);

            if (classID != 0)
            {
                allGames = allGames.Where(game => game.ClassId == classID);
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

        private List<AverageMainStats> AverageMainStatsCollector(int timeFrame, int classID = 0)
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

            var groupedPlayerGames = allGames.GroupBy(player => player.SteamId);

            var averagedStats = groupedPlayerGames.Select(
                player => new AverageMainStats(
                    player.Key,
                    player.Average(d => d.Damage / (d.Playtime / 60)),
                    player.Average(k => k.Kills),
                    player.Average(a => a.Assists),
                    player.Average(de => de.Deaths)
            ));

            return averagedStats.ToList();
        }

        private List<AverageAirshots> AverageAirshotCollector(int timeFrame, int classID = 0)
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

            var groupedPlayerGames = allGames.GroupBy(game => game.SteamId);

            var averagedAirshots = groupedPlayerGames.Select(player =>
                new AverageAirshots(
                    player.Key,
                    player.Average(a => a.Airshots)
                ));

            return averagedAirshots.ToList();
        }

        private List<AverageMedicStats> AverageMedicStatsCollector(int timeFrame)
        {
            var joinedPlayerStats = PlayerStatsJoinList();

            var allGames = joinedPlayerStats.Where(game =>
                game.Date > DateTimeOffset.Now.ToUnixTimeSeconds() - timeFrame &&
                game.ClassId == StatsCollectionConstants.MEDIC_CLASSID &&
                game.Playtime > StatsCollectionConstants.PLAYTIME_THRESHOLD);

            var groupedPlayerGames = allGames.GroupBy(game => game.SteamId);    

            var averagedMedicStats = groupedPlayerGames.Select(player =>
            new AverageMedicStats(
                player.Key,
                player.Average(d => d.Drops),
                player.Average(u => u.Ubers),
                player.Average(h => h.Heals / (h.Playtime / 60))
            ));

            return averagedMedicStats.ToList();
        }

        private List<AverageHeadshots> AverageHeadshotCollector(int timeFrame)
        {
            var joinedPlayerStats = PlayerStatsJoinList();

            var allGames = joinedPlayerStats.Where(game => 
                game.Date > DateTimeOffset.Now.ToUnixTimeSeconds() - timeFrame &&
                game.ClassId == StatsCollectionConstants.SNIPER_CLASSID
            );

            var groupedPlayerGames = allGames.GroupBy(game => game.SteamId);

            var averagedHeadshots = groupedPlayerGames.Select(player =>
                new AverageHeadshots(
                    player.Key,
                    player.Average(h => h.Headshots)
                ));

            return averagedHeadshots.ToList();
        }
        public List<PlayerPerformanceStats> RecentStats()
        {
            
            var playerNumberOfGames = PlayerGameCounter(StatsCollectionConstants.RECENTGAMES_THRESHOLD);
            var playerGeneralStats = AverageMainStatsCollector(StatsCollectionConstants.RECENTGAMES_THRESHOLD);
            var playerAirshots = AverageAirshotCollector(StatsCollectionConstants.RECENTGAMES_THRESHOLD);
            var playerHeadshots = AverageHeadshotCollector(StatsCollectionConstants.RECENTGAMES_THRESHOLD);

            var completeLeaderboard = (
                from png in playerNumberOfGames
                join pgs in playerGeneralStats on png.SteamId equals pgs.SteamId
                join pa in playerAirshots on png.SteamId equals pa.SteamId
                join ph in playerHeadshots on png.SteamId equals ph.SteamId
                where png.NumberOfGames > StatsCollectionConstants.PLAYER_NUMBEROFGAMES_THRESHOLD
                orderby pgs.AverageDPM descending
                select new PlayerPerformanceStats(
                    png.SteamId,
                    png.SteamName,
                    png.NumberOfGames,
                    pgs.AverageDPM,
                    pgs.AverageKills,
                    pgs.AverageAssists,
                    pgs.AverageDeaths,
                    pa.AirshotsAverage,
                    ph.HeadshotsAverage
                )
            );
            
            return completeLeaderboard.ToList();
        }

        public List<ScoutPerformanceStats> ScoutStatsAllTime()
        {
            var scoutGames = PlayerGameCounter(StatsCollectionConstants.ALLTIME_THRESHOLD, StatsCollectionConstants.SCOUT_CLASSID);
            var scoutPerformance = AverageMainStatsCollector(StatsCollectionConstants.ALLTIME_THRESHOLD, StatsCollectionConstants.SCOUT_CLASSID);

            var completeLeaderboard = (
                from sg in scoutGames
                join sp in scoutPerformance on sg.SteamId equals sp.SteamId
                where sg.NumberOfGames > StatsCollectionConstants.PLAYER_NUMBEROFGAMES_THRESHOLD
                orderby sp.AverageDPM descending
                select new ScoutPerformanceStats
                (
                    sg.SteamId,
                    sg.SteamName,
                    sg.NumberOfGames,
                    sp.AverageDPM,
                    sp.AverageKills,
                    sp.AverageAssists,
                    sp.AverageDeaths
                )
            );

            return completeLeaderboard.ToList();
        }

        public List<ScoutPerformanceStats> ScoutStatsRecent()
        {
            var scoutGames = PlayerGameCounter(StatsCollectionConstants.RECENTGAMES_THRESHOLD, StatsCollectionConstants.SCOUT_CLASSID);
            var scoutPerformance = AverageMainStatsCollector(StatsCollectionConstants.RECENTGAMES_THRESHOLD, StatsCollectionConstants.SCOUT_CLASSID);

            var completeLeaderboard = (
                from sg in scoutGames
                join sp in scoutPerformance on sg.SteamId equals sp.SteamId
                where sg.NumberOfGames > StatsCollectionConstants.PLAYER_NUMBEROFGAMES_THRESHOLD
                orderby sp.AverageDPM descending
                select new ScoutPerformanceStats
                (
                    sg.SteamId,
                    sg.SteamName,
                    sg.NumberOfGames,
                    sp.AverageDPM,
                    sp.AverageKills,
                    sp.AverageAssists,
                    sp.AverageDeaths
                )
            );

            return completeLeaderboard.ToList();
        }

        public List<ExplosiveClassStats> SoldierStatsAllTime()
        {
            var soldierGames = PlayerGameCounter(StatsCollectionConstants.ALLTIME_THRESHOLD, StatsCollectionConstants.SOLDIER_CLASSID);
            var SoldierPerformance = AverageMainStatsCollector(StatsCollectionConstants.ALLTIME_THRESHOLD, StatsCollectionConstants.SOLDIER_CLASSID);
            var soldierAirshots = AverageAirshotCollector(StatsCollectionConstants.ALLTIME_THRESHOLD,StatsCollectionConstants.SOLDIER_CLASSID);

            var completeLeaderboard = (
                from sg in soldierGames
                join sp in SoldierPerformance on sg.SteamId equals sp.SteamId
                join sa in soldierAirshots on sg.SteamId equals sa.SteamId
                where sg.NumberOfGames > StatsCollectionConstants.PLAYER_NUMBEROFGAMES_THRESHOLD
                orderby sp.AverageDPM descending
                select new ExplosiveClassStats
                (
                    sg.SteamId,
                    sg.SteamName,
                    sg.NumberOfGames,
                    sp.AverageDPM,
                    sp.AverageKills,
                    sp.AverageAssists,
                    sp.AverageDeaths,
                    sa.AirshotsAverage
                )
            );

            return completeLeaderboard.ToList();
        }

        public List<ExplosiveClassStats> SoldierStatsRecent()
        {
            var soldierGames = PlayerGameCounter(StatsCollectionConstants.RECENTGAMES_THRESHOLD, StatsCollectionConstants.SOLDIER_CLASSID);
            var SoldierPerformance = AverageMainStatsCollector(StatsCollectionConstants.RECENTGAMES_THRESHOLD, StatsCollectionConstants.SOLDIER_CLASSID);
            var soldierAirshots = AverageAirshotCollector(StatsCollectionConstants.RECENTGAMES_THRESHOLD,StatsCollectionConstants.SOLDIER_CLASSID);

            var completeLeaderboard = (
                from sg in soldierGames
                join sp in SoldierPerformance on sg.SteamId equals sp.SteamId
                join sa in soldierAirshots on sg.SteamId equals sa.SteamId
                where sg.NumberOfGames > StatsCollectionConstants.PLAYER_NUMBEROFGAMES_THRESHOLD
                orderby sp.AverageDPM descending
                select new ExplosiveClassStats
                (
                    sg.SteamId,
                    sg.SteamName,
                    sg.NumberOfGames,
                    sp.AverageDPM,
                    sp.AverageKills,
                    sp.AverageAssists,
                    sp.AverageDeaths,
                    sa.AirshotsAverage
                )
            );

            return completeLeaderboard.ToList();
        }

        public List<ExplosiveClassStats> DemomanStatsAllTime()
        {
            var demomanGames = PlayerGameCounter(StatsCollectionConstants.ALLTIME_THRESHOLD, StatsCollectionConstants.DEMOMAN_CLASSID);
            var demomanPerformance = AverageMainStatsCollector(StatsCollectionConstants.ALLTIME_THRESHOLD, StatsCollectionConstants.DEMOMAN_CLASSID);
            var demomanAirshots = AverageAirshotCollector(StatsCollectionConstants.ALLTIME_THRESHOLD,StatsCollectionConstants.DEMOMAN_CLASSID);

            var completeLeaderboard = (
                from dg in demomanGames
                join dp in demomanPerformance on dg.SteamId equals dp.SteamId
                join da in demomanAirshots on dg.SteamId equals da.SteamId
                where dg.NumberOfGames > StatsCollectionConstants.PLAYER_NUMBEROFGAMES_THRESHOLD
                orderby dp.AverageDPM descending
                select new ExplosiveClassStats
                (
                    dg.SteamId,
                    dg.SteamName,
                    dg.NumberOfGames,
                    dp.AverageDPM,
                    dp.AverageKills,
                    dp.AverageAssists,
                    dp.AverageDeaths,
                    da.AirshotsAverage
                )
            );

            return completeLeaderboard.ToList();
        }

        public List<ExplosiveClassStats> DemomanStatsRecent()
        {
            var demomanGames = PlayerGameCounter(StatsCollectionConstants.RECENTGAMES_THRESHOLD, StatsCollectionConstants.DEMOMAN_CLASSID);
            var demomanPerformance = AverageMainStatsCollector(StatsCollectionConstants.RECENTGAMES_THRESHOLD, StatsCollectionConstants.DEMOMAN_CLASSID);
            var demomanAirshots = AverageAirshotCollector(StatsCollectionConstants.RECENTGAMES_THRESHOLD,StatsCollectionConstants.DEMOMAN_CLASSID);

            var completeLeaderboard = (
                from dg in demomanGames
                join dp in demomanPerformance on dg.SteamId equals dp.SteamId
                join da in demomanAirshots on dg.SteamId equals da.SteamId
                where dg.NumberOfGames > StatsCollectionConstants.PLAYER_NUMBEROFGAMES_THRESHOLD
                orderby dp.AverageDPM descending
                select new ExplosiveClassStats
                (
                    dg.SteamId,
                    dg.SteamName,
                    dg.NumberOfGames,
                    dp.AverageDPM,
                    dp.AverageKills,
                    dp.AverageAssists,
                    dp.AverageDeaths,
                    da.AirshotsAverage
                )
            );

            return completeLeaderboard.ToList();
        }

        public List<MedicPerformanceStats> MedicStatsAllTime()
        {
            var medicGames = PlayerGameCounter(StatsCollectionConstants.ALLTIME_THRESHOLD, StatsCollectionConstants.MEDIC_CLASSID);
            var medicCombatStats = AverageMainStatsCollector(StatsCollectionConstants.ALLTIME_THRESHOLD, StatsCollectionConstants.MEDIC_CLASSID);
            var medicHealingStats = AverageMedicStatsCollector(StatsCollectionConstants.ALLTIME_THRESHOLD);

            var completeLeaderboard = (
                from mg in medicGames
                join mcs in medicCombatStats on mg.SteamId equals mcs.SteamId
                join mhs in medicHealingStats on mg.SteamId equals mhs.SteamId
                where mg.NumberOfGames > StatsCollectionConstants.PLAYER_NUMBEROFGAMES_THRESHOLD
                orderby mhs.AverageHeals descending
                select new MedicPerformanceStats(
                    mg.SteamId,
                    mg.SteamName,
                    mg.NumberOfGames,
                    mcs.AverageDPM,
                    mcs.AverageKills,
                    mcs.AverageAssists,
                    mcs.AverageDeaths,
                    mhs.AverageDrops,
                    mhs.AverageUbers,
                    mhs.AverageHeals
                )
            );

            return completeLeaderboard.ToList();
        }

        public List<MedicPerformanceStats> MedicStatsRecent()
        {
            var medicGames = PlayerGameCounter(StatsCollectionConstants.RECENTGAMES_THRESHOLD, StatsCollectionConstants.MEDIC_CLASSID);
            var medicCombatStats = AverageMainStatsCollector(StatsCollectionConstants.RECENTGAMES_THRESHOLD, StatsCollectionConstants.MEDIC_CLASSID);
            var medicHealingStats = AverageMedicStatsCollector(StatsCollectionConstants.RECENTGAMES_THRESHOLD);

            var completeLeaderboard = (
                from mg in medicGames
                join mcs in medicCombatStats on mg.SteamId equals mcs.SteamId
                join mhs in medicHealingStats on mg.SteamId equals mhs.SteamId
                where mg.NumberOfGames > StatsCollectionConstants.PLAYER_NUMBEROFGAMES_THRESHOLD
                orderby mhs.AverageHeals descending
                select new MedicPerformanceStats(
                    mg.SteamId,
                    mg.SteamName,
                    mg.NumberOfGames,
                    mcs.AverageDPM,
                    mcs.AverageKills,
                    mcs.AverageAssists,
                    mcs.AverageDeaths,
                    mhs.AverageDrops,
                    mhs.AverageUbers,
                    mhs.AverageHeals
                )
            );

            return completeLeaderboard.ToList();
        }
    }
    }

   