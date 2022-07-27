using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TF2SA.Data.Repositories.Base;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Services.Base;
using TF2SA.Data.Models;
using TF2SA.Data.Constants;
using TF2SA.Web.Models;

namespace TF2SA.Web.Controllers
{
    public class LeaderboardController : Controller
    {
        private readonly ILogger<LeaderboardController> _logger;
        private readonly IStatsService<ulong> statsService;

        public LeaderboardController(ILogger<LeaderboardController> logger, IStatsService<ulong> statsService)
        {
            _logger = logger;
            this.statsService = statsService;
        }

        public IActionResult AllTime()
        {
            var playerStats = statsService.AllTimeStats();
            return View(playerStats);
        }

        public IActionResult Recent()
        {
            var playerStats = statsService.RecentStats();
            return View(playerStats);
        }

        public IActionResult ScoutAllTime()
        {
            var scoutStats = statsService.ScoutStatsAllTime();
            return View(scoutStats);
        }

        public IActionResult ScoutRecent()
        {
            var scoutStats = statsService.ScoutStatsRecent();
            return View(scoutStats);
        }

        public IActionResult SoldierRecent()
        {
            var soldierStats = statsService.SoldierStatsRecent();
            return View(soldierStats);
        }

        public IActionResult SoldierAllTime()
        {
            var soldierStats = statsService.SoldierStatsAllTime(0);
            return View(soldierStats);
        }

        public IActionResult DemomanRecent()
        {
            var demoStats = statsService.DemomanStatsRecent();
            return View(demoStats);
        }

        public IActionResult DemomanAllTime()
        {
            var demoStats = statsService.DemomanStatsAllTime(0);
            return View(demoStats);
        }

        public IActionResult MedicAllTime()
        {
            var medicStats = statsService.MedicStatsAllTime(0);
            return View(medicStats);
        }

        public IActionResult MedicRecent()
        {
            var medicStats = statsService.MedicStatsRecent();
            return View(medicStats);
        }
        public IActionResult PlayerPage(ulong steamid)
        {
            var playerHighlights = statsService.PlayerHighlightCollector(steamid);
            var playerScoutStats = statsService.MainStatsCollector(StatsCollectionConstants.ALLTIME_THRESHOLD,1,steamid);
            var playerSoldierStats = statsService.SoldierStatsAllTime(steamid);
            var playerDemomanStats = statsService.DemomanStatsAllTime(steamid);
            var playerMedicStats = statsService.MedicStatsAllTime(steamid);

            var playerPageStats = new PlayerStatPageModel(playerHighlights,playerScoutStats,playerSoldierStats,playerDemomanStats,playerMedicStats);

            return View(playerPageStats);
        }
    }
}