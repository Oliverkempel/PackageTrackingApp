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

        // constructor som gør tilgang til services muligt
        public HomeController(IGmailService gmailService, ILogger<HomeController> logger, IMailHandler mailHandler, ITrackingInfo trackingInfo)
        {
            _gmailService = gmailService;
            _logger = logger;
            _mailHandler = mailHandler;
            _trackingInfo = trackingInfo;
        }

        //Returnere forsiden (Index siden)
        public IActionResult Index()
        {
            return View();
        }

        //Returnere Privacy siden
        public IActionResult Privacy()
        {
            return View();
        }


        [HttpGet]
        [Authorize]
        [Route("token")]
        public async Task<IActionResult> myPage()
        {
            //opretter en instans af klassen mailInfos og venter svar fra mailHandler services funktionen getAllTrackingNumbers() og sætter det derefter til instansen
            AllMailInfo mailInfos = await _mailHandler.getAllTrackingNumbers();

            //Initializere en liste af Shipments og tildeler den det der returneres fra funktionen getTrackingInfoAllCourriers, med parametren mailInfos.
            List<Shipment> allShipmentsFromUserInbox = _trackingInfo.getTrackingInfoAllCourriers(mailInfos);

            //En ny viewmodel initializeres, denne viewmodel er forventet af myPage viewet
            MyPageVM vm = new MyPageVM();

            //Shipments i viewmodel bliver derefter sat til de hentede shipment information hentet fra trackingInfo servicet.
            vm.shipments = allShipmentsFromUserInbox;

            //Der returneres til myPage viewet med viewdataen sendt med.
            return View(vm);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}