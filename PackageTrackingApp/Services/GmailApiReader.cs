using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authentication;

namespace PackageTrackingApp.Services;

public interface IGmailService
{
    Task<List<Message>> GetAllMails(string fromEmail);
}

public class GmailApiReader : IGmailService
{

    private readonly IHttpContextAccessor _httpContextAccessor;

    public GmailApiReader(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<List<Message>> GetAllMails(string fromEmail)
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
        //tilføjer et search query som søger efter mails fra mail "fromEmail" som indsættes fra funktions parameter
        emailListRequest.Q = $"from:{fromEmail}";
        //email requestet sendes og resultatet gemmes i emailListRespinse variablen
        var emailListResponse = await emailListRequest.ExecuteAsync();
        //kontrolstruktur der tjekker om svaret er null eller om der er nogle beskeder i emailListResponse
        if (emailListResponse?.Messages != null && emailListResponse.Messages.Any())
        {
            //hvis der er instanseres en ny liste af Message
            List<Message> messagesWithData = new List<Message>();

            //løkke der gennemgår hver message i emailListResponse
            foreach (var message in emailListResponse.Messages) { 

                // opbygger request til at hente selve beskeden med idet der tilhørende nuværende iteration i loopets id
                var emailInfoRequest = service.Users.Messages.Get("me", message.Id);
                //requestet sendes og svaret gemmes i variablen emailInfoResponse
                var emailInfoResponse = await emailInfoRequest.ExecuteAsync();
                // Dette tilføjes derefter til listen af messages
                messagesWithData.Add(emailInfoResponse);
            }

            //efter alle messages er tilføjet til listen messagesWithData returneres denne
            return messagesWithData;
        }
        else
        {
            //hvis der ingen messages er i resultatet returneres null
            return null;
        }
    }
}


