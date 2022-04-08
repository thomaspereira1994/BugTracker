using BugTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BugTracker.Controllers
{
    public class HomeController : Controller
    {
        #region PRIVATE VARIABLES
        private readonly ILogger<HomeController> _logger;

        #endregion

        #region CONSTRUCTOR
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        #endregion

        #region INDEX
        public IActionResult Index()
        {
            return View();
        }
        #endregion

        #region PRIVACY
        public IActionResult Privacy()
        {
            return View();
        }
        #endregion

        #region ERROR
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        } 
        #endregion
    }
}
