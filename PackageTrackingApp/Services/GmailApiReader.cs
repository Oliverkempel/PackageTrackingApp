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
    Task<string> GetLatestEmailAsync();
}

public class GmailApiReader : IGmailService
{

    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GmailApiReader(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<IList<Message>> GetLatestEmailAsync()
    {

        var authProps = await _httpContextAccessor.HttpContext.AuthenticateAsync();

        var accessToken = authProps.Properties.GetTokenValue("access_token");

        var credential = GoogleCredential.FromAccessToken(accessToken);
        
        var service = new GmailService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Gmail API Sample"
        });
        var emailListRequest = service.Users.Messages.List("me");
        emailListRequest.Q = "from:noreply@postnord.dk";
        var emailListResponse = await emailListRequest.ExecuteAsync();
        if (emailListResponse?.Messages != null && emailListResponse.Messages.Any())
        {
            foreach (var message in emailListResponse.Messages)
            {
                var email = emailListResponse.Messages.FirstOrDefault();
                var emailInfoRequest = service.Users.Messages.Get("me", email.Id);
                var emailInfoResponse = await emailInfoRequest.ExecuteAsync();
                var body = emailInfoResponse.Payload.Body.Data;
                Console.WriteLine(body);
            }

            return emailListResponse.Messages;
        }
        else
        {
            throw new Exception("no emails");
            return emailListResponse.Messages;
        }
    }
}


