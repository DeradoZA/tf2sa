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

namespace TF2SA.Web.Controllers
{
    [Route("[controller]")]
    public class LeaderboardController : Controller
    {
        private readonly ILogger<LeaderboardController> _logger;
        private readonly IStatsService statsService;

        public LeaderboardController(ILogger<LeaderboardController> logger, IStatsService statsService)
        {
            _logger = logger;
            this.statsService = statsService;
        }

        public IActionResult Index()
        {
            var playerStatsList = statsService.PlayerStatsJoinList();
            return View(playerStatsList);
        }

    }
}