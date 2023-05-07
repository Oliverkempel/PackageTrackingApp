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

    //interface til moetoden getAllTrackingNumbers
    public interface IMailHandler
    {
        Task<AllMailInfo> getAllTrackingNumbers();
    }

    public class MailHandler : IMailHandler
    {

        //Konstruktor til gmailService
        private readonly IGmailService _gmailService;
        public MailHandler(IGmailService gmailService)
        {
            _gmailService = gmailService;
        }


        //Get allTrackingNumbers metoden, er asynkron og har ingen parametre
        public async Task<AllMailInfo> getAllTrackingNumbers()
        {
            //initialisere ny liste af allMailinfos, som indeholder liste af måde postnord mails og gls mails
            var allMailInfos = new AllMailInfo
            {
                postNordMailInfos = new List<MailInfo>(),
                glsMailInfos = new List<MailInfo>(),
            };
            

            
            //initialisere ny liste af Message til opbevaring af postnord mails
            List<Message> postnordMessages = new List<Message>();
            //Henter postnord messages via GmailApiReader servicen
            postnordMessages = await _gmailService.GetAllMails("noreply@postnord.dk");

            
            if (postnordMessages != null)
            {
                //looper igennem mails hentet fra brugerens gmail, og gemmer dem i mailinfos
                foreach (Message message in postnordMessages)
                {
                    //instans af Mailinfo til opbevaring af data hentet fra mail
                    MailInfo currentMailinfo = new MailInfo();
                    // currntMailInfo tildeles informationer hentet fra mail via getTrackingNumberPostnord metoden
                    currentMailinfo = getTrackingNumberPostnord(message);

                    // Kontrolstruktur sikrer at trackingnummeret ikke er en tom streng.
                    if (currentMailinfo.trackingNumber != "")
                    {
                        //tilføjer den opbyggede mailinfo til listen af postnordmailinfo i allMailInfos-¨.
                        allMailInfos.postNordMailInfos.Add(currentMailinfo);
                    }

                }
            }
            

            //Initialisere ny af Message til opbevaring af gls messages fra gmailServicen
            List<Message> glsMessages = new List<Message>();
            // tildeles mails hentet fra GetLatestEmailAsync funktionen fra gmailsericen, med email parametren noreply@glsdanmark.dk, som sørger for det kun er mails med afsender der mather dette der hentes
            glsMessages = await _gmailService.GetAllMails("noreply@glsdanmark.dk");

            //tjekker at glsMessages ikke er null
            if(glsMessages != null)
            {
                //looper igennem Messages i GlsMessages
                foreach (Message message in glsMessages)
                {
                    //henter trackingnummeret fra gls "Messagen" og tilføjer informationen hentet i GlsMailInfos listen i allMailInfos
                    allMailInfos.glsMailInfos.Add(getTrackingNumberGls(message));
                }
            }

            //returnere allMailInfos
            return allMailInfos;

        }

        public MailInfo getTrackingNumberPostnord(Message Mail)
        {
            //Initialisere ny instans af HtmlDocument
            HtmlDocument htmlDoc = new HtmlDocument();
            //Loader string til Html filen, som bliver decoded fra Base64Url
            htmlDoc.LoadHtml(Base64UrlEncoder.Decode(Mail.Payload.Body.Data));

            //gemmer datoen som mailen er modtaget i en DateTime variabel
            DateTime dateRecieved = DateTime.Parse(Mail.Payload.Headers.Where(x => x.Name == "Date").First().Value);
            //vælger html tag med klassen "KONSTANT" og vælger teksten deri
            var tagInnerText = htmlDoc.DocumentNode.SelectSingleNode("//*[@class='KONSTANT']").InnerText;

            //opbygger regEx mønstret som skal søges efter.
            string regExPattern = @"(?<=Dit pakkenummer er: )\w+";

            //instansere regex
            Regex reg = new Regex(regExPattern);

            //bruger regex tagInnerText og gemmer den som det enedlige trackingnummer.
            string trackingNumber = reg.Match(tagInnerText).ToString();

            //laver ny instans af mailInfo
            MailInfo mailInfo = new MailInfo();

            //Gemmer oplysningerne i mailInfo
            mailInfo.Courier = "postnord";
            mailInfo.trackingNumber = trackingNumber;
            mailInfo.mailRecieveDate = dateRecieved;

            //Returnere mailInfo fra metoden
            return mailInfo;

        }

        public MailInfo getTrackingNumberGls(Message Mail)
        {
           
            //Decoder emailens body fra base64
            string message = Base64UrlEncoder.Decode(Mail.Payload.Parts.First().Body.Data);

            // gemmer regexstring i variabel
            string regExPattern = @"(?<=txtRefNo=)\w*";

            // instansere regex
            Regex reg = new Regex(regExPattern);

            // henter trackingnummer med regex.
            string trackingNumber = reg.Match(message).ToString();

            //laver ny instans af mailInfo
            MailInfo mailInfo = new MailInfo();
            
            //gemmer datoen fra mailen i en string
            string dateString = Mail.Payload.Headers.Where(x => x.Name == "Date").First().Value;

            //omdanner datoen til Datetime
            DateTime dateRecieved = DateTime.Parse(dateString.Substring(0, dateString.Length - 6));

            //gemmer oplysningerne i mailInfo
            mailInfo.Courier = "gls";
            mailInfo.trackingNumber = trackingNumber;
            mailInfo.mailRecieveDate = dateRecieved; 

            //returnere mailInfo
            return mailInfo;
        }

    }

}