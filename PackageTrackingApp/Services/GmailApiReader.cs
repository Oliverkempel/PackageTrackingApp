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
    Task<List<Message>> GetLatestEmailAsync(string fromEmail);
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
    public async Task<List<Message>> GetLatestEmailAsync(string fromEmail)
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
        emailListRequest.Q = $"from:{fromEmail}";
        var emailListResponse = await emailListRequest.ExecuteAsync();
        if (emailListResponse?.Messages != null && emailListResponse.Messages.Any())
        {
            List<Message> messagesWithData = new List<Message>();

            foreach (var message in emailListResponse.Messages)
            {
                var email = emailListResponse.Messages.FirstOrDefault();
                var emailInfoRequest = service.Users.Messages.Get("me", email.Id);
                var emailInfoResponse = await emailInfoRequest.ExecuteAsync();
                messagesWithData.Add(emailInfoResponse);
                //var body = emailInfoResponse.Payload.Body.Data;
                //Console.WriteLine(body);
            }

            return messagesWithData;
        }
        else
        {
            return null;
        }
    }
}


