using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TF2SA.Web.Models;
using TF2SA.Data;

namespace TF2SA.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly TF2SADbContext _TF2SADbContext;

    public HomeController(ILogger<HomeController> logger, TF2SADbContext TF2SADbContext)
    {
        _logger = logger;
        _TF2SADbContext = TF2SADbContext;
    }

    public IActionResult Index()
    {
        var players = _TF2SADbContext.Players.AsEnumerable();
        _logger.LogInformation($" Players: {players.Count()}");
        return View(players);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
