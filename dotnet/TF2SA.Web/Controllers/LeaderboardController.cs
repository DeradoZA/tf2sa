using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TF2SA.Web.Models;
using TF2SA.Data.Repositories.Base;
using TF2SA.Data.Entities.MariaDb;

namespace TF2SA.Web.Controllers
{
    [Route("[controller]")]
    public class LeaderboardController : Controller
    {
        private readonly ILogger<LeaderboardController> _logger;
        private readonly IPlayersRepository<Player, ulong> PlayerRepository;

        public LeaderboardController(ILogger<LeaderboardController> logger, IPlayersRepository<Player, ulong> PlayerRepository)
        {
            _logger = logger;
            this.PlayerRepository = PlayerRepository;
        }

        public IActionResult Index()
        {
            var PlayerList = PlayerRepository.GetAll();
            return View(PlayerList);
        }

    }
}