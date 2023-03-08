using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace PackageTrackingApp.Services;
public class TokenProvider
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}