﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PackageTrackingApp.Models;
using PackageTrackingApp.Services;
using System.Diagnostics;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

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
        [HttpGet]
        [Authorize]
        [Route("token")]
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