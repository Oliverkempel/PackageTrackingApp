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
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

namespace PackageTrackingApp.Services
{


    public interface IMailHandler
    {
        Task<AllMailInfo> getAllTrackingNumbers();
    }

    public class MailHandler : IMailHandler
    {

        private readonly IGmailService _gmailService;


        public MailHandler(IGmailService gmailService, ILogger<HomeController> logger)
        {
            _gmailService = gmailService;
        }

        public async Task<AllMailInfo> getAllTrackingNumbers()
        {
            //initialisere ny liste af allMailinfos
            var allMailInfos = new AllMailInfo
            {
                postNordMailInfos = new List<MailInfo>(),
                glsMailInfos = new List<MailInfo>(),
            };
            

            
            //initialisere ny liste af Message til opbevaring af postnord mails
            List<Message> postnordMessages = new List<Message>();
            //Henter postnord messages via GmailApiReader servicen
            postnordMessages = await _gmailService.GetLatestEmailAsync("noreply@postnord.dk");
            //looper igennem mails hentet fra brugerens gmail, og gemmer dem i mailinfos
            foreach (Message message in postnordMessages)
            {
                MailInfo test = new MailInfo();
                test = getTrackingNumberPostnord(message);

                if(test.trackingNumber != "")
                {
                    allMailInfos.postNordMailInfos.Add(test);
                }

            }

            List<Message> glsMessages = new List<Message>();
            glsMessages = await _gmailService.GetLatestEmailAsync("noreply@glsdanmark.dk");

            foreach(Message message in glsMessages)
            {
                allMailInfos.glsMailInfos.Add(getTrackingNumberGls(message));
            }

            return allMailInfos;

        }

        public MailInfo getTrackingNumberPostnord(Message Mail)
        {
            //Initialisere ny instans af HtmlDocument
            HtmlDocument htmlDoc = new HtmlDocument();
            //Loader string til Html filen, som bliver decoded fra Base64Url
            htmlDoc.LoadHtml(Base64UrlEncoder.Decode(Mail.Payload.Body.Data));

            DateTime dateRecieved = DateTime.Parse(Mail.Payload.Headers.Where(x => x.Name == "Date").First().Value);
            //vælger tag med klassen "KONSTANT" og vælger teksten deri
            var tagInnerText = htmlDoc.DocumentNode.SelectSingleNode("//*[@class='KONSTANT']").InnerText;

            string regExPattern = @"(?<=Dit pakkenummer er: )\w+";

            Regex reg = new Regex(regExPattern);

            string trackingNumber = reg.Match(tagInnerText).ToString();

            MailInfo mailInfo = new MailInfo();

            mailInfo.Courier = "postnord";
            mailInfo.trackingNumber = trackingNumber;
            mailInfo.mailRecieveDate = dateRecieved;


            return mailInfo;

        }

        public MailInfo getTrackingNumberGls(Message Mail)
        {
           
            string message = Base64UrlEncoder.Decode(Mail.Payload.Parts.First().Body.Data);

            string regExPattern = @"(?<=txtRefNo=)\w*";

            Regex reg = new Regex(regExPattern);

            string trackingNumber = reg.Match(message).ToString();

            MailInfo mailInfo = new MailInfo();


            string dateString = Mail.Payload.Headers.Where(x => x.Name == "Date").First().Value;
            DateTime dateRecieved = DateTime.Parse(dateString.Substring(0, dateString.Length - 6));

            mailInfo.Courier = "gls";
            mailInfo.trackingNumber = trackingNumber;
            mailInfo.mailRecieveDate = dateRecieved; 

            return mailInfo;
        }

    }

}