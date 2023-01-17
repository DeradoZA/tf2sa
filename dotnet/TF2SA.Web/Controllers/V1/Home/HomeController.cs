using Microsoft.AspNetCore.Mvc;

namespace TF2SA.Web.Controllers.v1.Home;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
	private readonly ILogger<HomeController> logger;

	public HomeController(ILogger<HomeController> logger)
	{
		this.logger = logger;
	}

	public IActionResult Index()
	{
		return Ok("OK");
	}
}
