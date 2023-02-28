// using SendGrid's C# Library
// https://github.com/sendgrid/sendgrid-csharp
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace PackageTrackingApp.Services
{
    public class ConfirmEmail
    {
        public static async void run()
        {
            await Execute();
        }
        
        static async Task Execute()
        {
            var apiKey = "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";
            Console.WriteLine(apiKey);
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("mads.gjellerod@gmail.com", "Password Recovery");
            var subject = "Sending with SendGrid is Fun";
            var to = new EmailAddress("mads.gjellerod@gmail.com", "Example User");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            Console.WriteLine(response.StatusCode);
            Console.WriteLine(response.Body.ReadAsStringAsync());
        }
    }
}   