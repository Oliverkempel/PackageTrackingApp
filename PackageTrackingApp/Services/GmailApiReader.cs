using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;


using PackageTrackingApp.Models;

namespace PackageTrackingApp.Services;

public interface IGmailService
{
    Task<List<Message>> getAllEmails(string fromEmail);
}

public class GmailApiReader : IGmailService
{

    private readonly IHttpContextAccessor _httpContextAccessor;

    public GmailApiReader(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<List<Message>> getAllEmails(string fromEmail)
    {
        // henter Auth propertires indeholdenede accesstoken asynkront og gemmer det i authProps
        var authProps = await _httpContextAccessor.HttpContext.AuthenticateAsync();

        // gemmer accessToken i variabel fra authprops
        var accessToken = authProps.Properties.GetTokenValue("access_token");

        //omdanner accesstoken til google credential
        var credential = GoogleCredential.FromAccessToken(accessToken);
        
        //opretter ny instans af Gmailservice
        var service = new GmailService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Gmail API Sample"
        });

        //opretter en request til at hente mails fra brugeren
        var emailListRequest = service.Users.Messages.List("me");
        //tilf�jer et search query som s�ger efter mails fra mail "fromEmail" som inds�ttes fra funktions parameter
        emailListRequest.Q = $"from:{fromEmail}";
        //email requestet sendes og resultatet gemmes i emailListRespinse variablen
        var emailListResponse = await emailListRequest.ExecuteAsync();
        //kontrolstruktur der tjekker om svaret er null eller om der er nogle beskeder i emailListResponse
        if (emailListResponse?.Messages != null && emailListResponse.Messages.Any())
        {
            //hvis der er instanseres en ny liste af Message
            List<Message> messagesWithData = new List<Message>();

            //l�kke der gennemg�r hver message i emailListResponse
            foreach (var message in emailListResponse.Messages) { 

                // opbygger request til at hente selve beskeden med idet der tilh�rende nuv�rende iteration i loopets id
                var emailInfoRequest = service.Users.Messages.Get("me", message.Id);
                //requestet sendes og svaret gemmes i variablen emailInfoResponse
                var emailInfoResponse = await emailInfoRequest.ExecuteAsync();
                // Dette tilf�jes derefter til listen af messages
                messagesWithData.Add(emailInfoResponse);
            }

            //efter alle messages er tilf�jet til listen messagesWithData returneres denne
            return messagesWithData;
        }
        else
        {
            //hvis der ingen messages er i resultatet returneres null
            return null;
        }
    }
}


