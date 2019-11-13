using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PingPong.Repository;
using PingPong.Types;
using PingPong.Webs.Models;

namespace PingPong.Webs.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ITeamRepository _teamRepo;
        private ITeamRepository TeamRepo
                => _teamRepo ??= new FileTeamRepository();
        //=> _teamRepo ??= new FakeTeamRepository();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public ActionResult Welcome(string name, string email, int shirtSize)
        {
            var color = TeamRepo.AddPlayer(name, email, (ShirtSizeType)shirtSize);
            switch (color)
            {
                case TeamType.Red:
                    return View(new RedWelcomeModel());
                case TeamType.Black:
                    return View(new BlackWelcomeModel());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public IActionResult Teams()
        {
            var teamsModel = TeamRepo.GetPlayers();
            if (teamsModel.RedTeam.Count > teamsModel.BlackTeam.Count)
            {
                teamsModel.BlackTeam.Add(string.Empty);
            }
            else if (teamsModel.BlackTeam.Count > teamsModel.RedTeam.Count)
            {
                teamsModel.RedTeam.Add(string.Empty);
            }
            return View(teamsModel);
        }

        [HttpGet]
        public JsonResult GetPlayers()
        {
            var teamsModel = TeamRepo.GetPlayers();

            return new JsonResult(teamsModel);
        }

    }
}
