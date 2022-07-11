using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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

        private readonly ILogger<StatsService> logger;
        
        public StatsService(
            IPlayersRepository<Player, ulong> playerRepository,
            IPlayerStatsRepository<PlayerStat, uint> playerStatRepository,
            IClassStatsRepository<ClassStat, uint> classStatsRepository,
            ILogger<StatsService> logger)
        {
            this.playerRepository = playerRepository;
            this.playerStatRepository = playerStatRepository;
            this.classStatsRepository = classStatsRepository;
            this.logger = logger;
        }

        public IQueryable<JoinedStats> PlayerStatsJoinQueryable()
        {
            var players = playerRepository.GetAllQueryable();
            var playerStats = playerStatRepository.GetAllQueryable();
            var classStats = classStatsRepository.GetAllQueryable();

            var InnerJoinQuery = 
                from player in players
                join playerStat in playerStats on player.SteamId equals playerStat.SteamId
                join classStat in classStats on playerStat.PlayerStatsId equals classStat.PlayerStatsId
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
                        classStat.Damage
                    
                    );

            return InnerJoinQuery;
        }

        public List<JoinedStats> PlayerStatsJoinList()
        {
            return PlayerStatsJoinQueryable().ToList();
        }

        public List<AllTimeStats> AllTimeStats()
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
                where jps.ClassId != 7
                group jps by jps.SteamId into groupedPlayerStats
                select new
                {
                    SteamID = groupedPlayerStats.Key,
                    SteamName = groupedPlayerStats.FirstOrDefault().PlayerName,
                    AverageDPM = groupedPlayerStats.Average(d => d.Damage),
                    AverageKills = groupedPlayerStats.Average(k => k.Kills),
                    AverageAssists = groupedPlayerStats.Average(a => a.Assists),
                    AverageDeaths = groupedPlayerStats.Average(de => de.Deaths)
                }
            ).ToList();

            var playerAirshots = ( 
                from jps in joinedPlayerStats
                where jps.ClassId == 2 || jps.ClassId == 4
                group jps by jps.SteamId into groupedPlayerStats
                select new
                {
                    SteamID = groupedPlayerStats.Key,
                    AverageAirshots = groupedPlayerStats.Average(a => a.Airshots)
                }
            ).ToList();

            var completeLeaderboard = (
                from png in playerNumGames
                join ps in playerStats on png.SteamID equals ps.SteamID
                join pa in playerAirshots on png.SteamID equals pa.SteamID
                where png.NumberOfGames > 20
                orderby ps.AverageDPM descending
                select new AllTimeStats
                (
                    png.SteamID,
                    ps.SteamName,
                    png.NumberOfGames,
                    ps.AverageDPM,
                    ps.AverageKills,
                    ps.AverageAssists,
                    ps.AverageDeaths,
                    pa.AverageAirshots
                )
            );

            return completeLeaderboard.ToList();
        }

}
}

   