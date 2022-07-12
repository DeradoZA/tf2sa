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
            var allPlayerGames = playerStatRepository.GetAll();
            var joinedPlayerStats = PlayerStatsJoinList();

            var playerNumGames = (
                from playerGame in allPlayerGames
                group playerGame by playerGame.SteamId into groupedPlayerGames
                select new
                {
                    SteamID = groupedPlayerGames.Key,
                    NumberOfGames = groupedPlayerGames.Count(),
                }
            ).ToList();

            var playerStats = (
                from jps in joinedPlayerStats
                where jps.ClassId != StatsCollectionConstants.Medic_ClassID && jps.Playtime > StatsCollectionConstants.Playtime_Threshold
                group jps by jps.SteamId into groupedPlayerStats
                select new
                {
                    SteamID = groupedPlayerStats.Key,
                    SteamName = groupedPlayerStats.FirstOrDefault().PlayerName,
                    AverageDPM = groupedPlayerStats.Average(d => d.Damage / (d.Playtime / 60)),
                    AverageKills = groupedPlayerStats.Average(k => k.Kills),
                    AverageAssists = groupedPlayerStats.Average(a => a.Assists),
                    AverageDeaths = groupedPlayerStats.Average(de => de.Deaths)
                }
            ).ToList();

            var playerAirshots = ( 
                from jps in joinedPlayerStats
                where jps.ClassId == StatsCollectionConstants.Soldier_ClassID || jps.ClassId == StatsCollectionConstants.Demoman_ClassID
                group jps by jps.SteamId into groupedPlayerStats
                select new
                {
                    SteamID = groupedPlayerStats.Key,
                    AverageAirshots = groupedPlayerStats.Average(a => a.Airshots)
                }
            ).ToList();

            var playerHeadshots = (
                from jps in joinedPlayerStats
                where jps.ClassId == StatsCollectionConstants.Sniper_ClassID
                group jps by jps.SteamId into groupedPlayerStats
                select new
                {
                        SteamID = groupedPlayerStats.Key,
                        AverageHeadshots = groupedPlayerStats.Average(h => h.Headshots)
                }
            );

            var completeLeaderboard = (
                from png in playerNumGames
                join ps in playerStats on png.SteamID equals ps.SteamID
                join pa in playerAirshots on png.SteamID equals pa.SteamID
                join ph in playerHeadshots on png.SteamID equals ph.SteamID
                where png.NumberOfGames > StatsCollectionConstants.Player_NumberOfGames_Threshold
                orderby ps.AverageDPM descending
                select new PlayerPerformanceStats
                (
                    png.SteamID,
                    ps.SteamName,
                    png.NumberOfGames,
                    ps.AverageDPM,
                    ps.AverageKills,
                    ps.AverageAssists,
                    ps.AverageDeaths,
                    pa.AverageAirshots,
                    ph.AverageHeadshots
                )
            );

            return completeLeaderboard.ToList();
        }

        public List<PlayerPerformanceStats> RecentStats()
        {
            var allPlayerGames = playerStatRepository.GetAll();
            var joinedPlayerStats = PlayerStatsJoinList();

            var playerNumGames = (
            from jps in joinedPlayerStats
            where jps.Date > DateTimeOffset.Now.ToUnixTimeSeconds() - StatsCollectionConstants.RecentGames_Threshold
            group jps by jps.SteamId into groupedPlayerGames
            select new
            {
                SteamID = groupedPlayerGames.Key,
                NumberOfGames = groupedPlayerGames.Select(g => g.PlayerStatsId).Distinct().Count()
            }).ToList();

            var playerStats = (
                from jps in joinedPlayerStats
                where jps.ClassId != StatsCollectionConstants.Medic_ClassID && jps.Playtime > StatsCollectionConstants.Playtime_Threshold
                && jps.Date > DateTimeOffset.Now.ToUnixTimeSeconds() - StatsCollectionConstants.RecentGames_Threshold
                group jps by jps.SteamId into groupedPlayerStats
                select new
                {
                    SteamID = groupedPlayerStats.Key,
                    SteamName = groupedPlayerStats.FirstOrDefault().PlayerName,
                    AverageDPM = groupedPlayerStats.Average(d => d.Damage / (d.Playtime / 60)),
                    AverageKills = groupedPlayerStats.Average(k => k.Kills),
                    AverageAssists = groupedPlayerStats.Average(a => a.Assists),
                    AverageDeaths = groupedPlayerStats.Average(de => de.Deaths)
                }
            ).ToList();

            var playerAirshots = ( 
                from jps in joinedPlayerStats
                where (jps.ClassId == StatsCollectionConstants.Soldier_ClassID || jps.ClassId == StatsCollectionConstants.Demoman_ClassID)
                && (jps.Date > DateTimeOffset.Now.ToUnixTimeSeconds() - StatsCollectionConstants.RecentGames_Threshold)
                group jps by jps.SteamId into groupedPlayerStats
                select new
                {
                    SteamID = groupedPlayerStats.Key,
                    AverageAirshots = groupedPlayerStats.Average(a => a.Airshots)
                }
            ).ToList();

             var playerHeadshots = (
                from jps in joinedPlayerStats
                where jps.ClassId == StatsCollectionConstants.Sniper_ClassID
                && (jps.Date > DateTimeOffset.Now.ToUnixTimeSeconds() - StatsCollectionConstants.RecentGames_Threshold)
                group jps by jps.SteamId into groupedPlayerStats
                select new
                {
                        SteamID = groupedPlayerStats.Key,
                        AverageHeadshots = groupedPlayerStats.Average(h => h.Headshots)
                }
            ).ToList();

            var completeLeaderboard = (
                from png in playerNumGames
                join ps in playerStats on png.SteamID equals ps.SteamID
                join pa in playerAirshots on png.SteamID equals pa.SteamID
                join ph in playerHeadshots on png.SteamID equals ph.SteamID
                where png.NumberOfGames > StatsCollectionConstants.Player_NumberOfGames_Threshold
                orderby ps.AverageDPM descending
                select new PlayerPerformanceStats
                (
                    png.SteamID,
                    ps.SteamName,
                    png.NumberOfGames,
                    ps.AverageDPM,
                    ps.AverageKills,
                    ps.AverageAssists,
                    ps.AverageDeaths,
                    pa.AverageAirshots,
                    ph.AverageHeadshots
                )
            );

            return completeLeaderboard.ToList();
        }
    }
}

   