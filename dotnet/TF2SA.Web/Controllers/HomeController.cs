using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;
using TF2SA.Web.Models;

namespace TF2SA.Web.Controllers;

public class HomeController : Controller
{
	private readonly ILogger<HomeController> logger;
	private readonly IPlayersRepository<Player, ulong> playerRepository;

	public HomeController(
		ILogger<HomeController> logger,
		IPlayersRepository<Player, ulong> playerRepository
	)
	{
		this.logger = logger;
		this.playerRepository = playerRepository;
	}

	public IActionResult Index()
	{
		var players = playerRepository.GetAll();
		logger.LogInformation("Players: {playerCount}", players.Count);
		return View(players);
	}

	public IActionResult Privacy()
	{
		return View();
	}

	[ResponseCache(
		Duration = 0,
		Location = ResponseCacheLocation.None,
		NoStore = true
	)]
	public IActionResult Error()
	{
		return View(
			new ErrorViewModel
			{
				RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
			}
		);
	}
}
