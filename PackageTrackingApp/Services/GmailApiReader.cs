using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;  

namespace PackageTrackingApp.Services;

public class GmailApiReader
{        
    public async Task<string> GetLatestEmailAsync()
    {
        return "no email";
    }
}
