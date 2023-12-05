using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using NoteaAPI.Models.ErrorModels;

namespace NoteaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [HttpPost]
        [Route("index")]
        public IActionResult Index()
        {
            return Ok();
        }
        [HttpPost]
        [Route("privacy")]
        public IActionResult Privacy()
        {
            return Ok();
        }
    }
}
