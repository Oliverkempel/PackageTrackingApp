using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PackageTrackingApp.Models;
using PackageTrackingApp.Services;
using System.Diagnostics;

namespace PackageTrackingApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGmailService _gmailService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IGmailService gmailService, ILogger<HomeController> logger)
        {
            _gmailService = gmailService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Test()
        {
            try
            {
                var latestEmail = await _gmailService.GetLatestEmailAsync();
                _logger.LogInformation($"Latest email body: {latestEmail}");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the latest email.");
                return StatusCode(500, "An error occurred while retrieving the latest email.");
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}