using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TF2SA.Data.Repositories.Base;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Web.Models;

namespace TF2SA.Web.Controllers
{
    [Route("[controller]")]
    public class LeaderBoardController : Controller
    {
        private readonly ILogger<LeaderBoardController> _logger;
        private readonly IPlayersRepository<Player, ulong> playersRepository;

        public LeaderBoardController(ILogger<LeaderBoardController> logger, IPlayersRepository<Player, ulong> playersRepository)
        {
            _logger = logger;
            this.playersRepository = playersRepository;
        }

        public IActionResult Index()
        {
            var playerList = playersRepository.GetAll();

            return View(playerList);
        }
        
     } 


    }
