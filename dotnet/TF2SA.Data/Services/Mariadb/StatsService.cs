using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public StatsService(
            IPlayersRepository<Player, ulong> playerRepository,
            IPlayerStatsRepository<PlayerStat, uint> playerStatRepository,
            IClassStatsRepository<ClassStat, uint> classStatsRepository)
        {
            this.playerRepository = playerRepository;
            this.playerStatRepository = playerStatRepository;
            this.classStatsRepository = classStatsRepository;
        }

        public IQueryable<JoinedStats> PlayerStatsJoinQueryable()
        {
            var players = playerRepository.GetAllQueryable();
            var playerStats = playerStatRepository.GetAllQueryable();
            var classStats = classStatsRepository.GetAllQueryable();

            var InnerJoinQuery = from player in players
                                 join playerStat in playerStats on player.SteamId equals playerStat.SteamId
                                 join classStat in classStats on playerStat.PlayerStatsId equals classStat.PlayerStatsId
                                 select new JoinedStats(player.SteamId, player.PlayerName, playerStat.PlayerStatsId,
                                             playerStat.GameId, playerStat.TeamId, playerStat.DamageTaken,
                                             playerStat.HealsReceived, playerStat.MedkitsHp, playerStat.Airshots,
                                             playerStat.Headshots, playerStat.Backstabs, playerStat.Drops,
                                             playerStat.Heals, playerStat.Ubers, classStat.ClassStatsId,
                                             classStat.ClassId, classStat.Playtime, classStat.Kills,
                                             classStat.Assists, classStat.Deaths, classStat.Damage);

            return InnerJoinQuery;
        }

        public List<JoinedStats> PlayerStatsJoinList()
        {
            return PlayerStatsJoinQueryable().ToList();
        }

      
}
}

   