using PackageTrackingApp.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;
using PackageTrackingApp.Services;
using PackageTrackingApp.Controllers;

namespace PackageTrackingApp.Services; 

    public interface IMailHandler
    {
        void test();
    }

    public class MailHandler : IMailHandler
    {

        private readonly IGmailService _gmailService;
        

        public MailHandler(IGmailService gmailService, ILogger<HomeController> logger)
        {
            _gmailService = gmailService;
        }

        public async void test()
        {
            List<Message> messages = new List<Message>();
            messages = await _gmailService.GetLatestEmailAsync();
        foreach (Message message in messages)
        {
            string trackingnum = getTrackingNumberPostnord(message);
        }
            
        


            Console.WriteLine("CUM");
        }

        public string getTrackingNumberPostnord(Message Mail)
        {
            //Initialisere ny instans af HtmlDocument
            HtmlDocument htmlDoc = new HtmlDocument();
            //Loader string til Html filen
            htmlDoc.LoadHtml(Convert.FromBase64String(Mail.Payload.Body.Data).ToString());

            //vælger tag med klassen "KONSTANT" og vælger teksten deri
            var tagInnerText = htmlDoc.DocumentNode.SelectSingleNode("//*[@class='KONSTANT']").InnerText;

            string regExPattern = @"(?<=Dit pakkenummer er: )\w+";

            Regex reg = new Regex(regExPattern);

            string trackingNumber = reg.Match(tagInnerText).ToString();
            // vælger trackingnummer
            //string trackingNumber = tagInnerText.Split(" ").ElementAt(3).Split(".").First();


            return trackingNumber;

        }

        public string getTrackingNumberGls(Message Mail)
        {
            //string glsMail = "Tak fordi du handlede hos MaPerle. Din pakke er afsendt med GLS.\r\n\r\nNår du kan hente pakken, modtager du besked på mail/sms fra GLS.\r\n\r\nDu kan følge pakken frem til dig via GLS Track\r\n& Trace: http://www.gls-group.eu/276-I-PORTAL-WEB/content/GLS/DK01/DA/5004.htm?txtRefNo=YO4T9ISL&txtAction=71000.\r\n\r\nVenlig hilsen\r\n\r\nGLS\r\n\r\nwww.gls-group.eu\r\n\r\n\r\nØnsker du fremover ikke at modtage denne type advisering, kan du  afmelde dig her: http://gls.dk/unsubscribe/unsubscribe.aspx?key=5d89c1e8-91f1-4ba8-9ca7-cb677d5caa58&mail=oliversbutik@gmail.com.";
            //string glsMail2 = "Tak fordi du handlede hos Autofix.Nu ApS. Din pakke er afsendt med GLS.\r\n\r\nNår du kan hente pakken, modtager du besked på mail/sms fra GLS.\r\n\r\nDu kan følge pakken frem til dig via GLS Track\r\n& Trace: http://www.gls-group.eu/276-I-PORTAL-WEB/content/GLS/DK01/DA/5004.htm?txtRefNo=YORVF8L5&txtAction=71000.\r\n\r\nVenlig hilsen\r\n\r\nGLS\r\n\r\nwww.gls-group.eu\r\n\r\n\r\nØnsker du fremover ikke at modtage denne type advisering, kan du  afmelde dig her: http://gls.dk/unsubscribe/unsubscribe.aspx?key=f6db6ad6-7d3e-49d5-bd78-b3ea79fb9ef1&mail=Oliversbutik@gmail.com.";

            string regExPattern = @"(?<=txtRefNo=)\w*";

            Regex reg = new Regex(regExPattern);

            string trackingNumber = reg.Match(Mail.Payload.Body.Data).ToString();


            //string trackingNumber = Mail.Split("\t\n ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Where(s => s.StartsWith("http://")).First().Split("/").ElementAt(8).Split("=").ElementAt(1).Split("&").First();

            return trackingNumber;
        }

    }
