using Microsoft.AspNetCore.Authorization;
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
using PackageTrackingApp.Services;
using PackageTrackingApp.Viewmodels;

namespace PackageTrackingApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGmailService _gmailService;
        private readonly ILogger<HomeController> _logger;
        private readonly IMailHandler _mailHandler;
        private readonly ITrackingInfo _trackingInfo;

        public HomeController(IGmailService gmailService, ILogger<HomeController> logger, IMailHandler mailHandler, ITrackingInfo trackingInfo)
        {
            _gmailService = gmailService;
            _logger = logger;
            _mailHandler = mailHandler;
            _trackingInfo = trackingInfo;
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
            AllMailInfo mailInfos = await _mailHandler.getAllTrackingNumbers();

            List<Shipment> allShipmentsFromUserInbox = _trackingInfo.getTrackingInfoAllCourriers(mailInfos);

            MyPageVM vm = new MyPageVM();

            vm.shipments = allShipmentsFromUserInbox;
            //var test2 = _trackingInfo.getPostnordTrackingInfo("05308115208628");

            //var latestEmail = await _gmailService.GetLatestEmailAsync("noreply@postnord.dk");
            //_logger.LogInformation($"Latest email body: {latestEmail}");
            return View(vm);
            //try
            //{
                
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "An error occurred while retrieving the latest email.");
            //    return StatusCode(500, "An error occurred while retrieving the latest email.");
            //}
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}