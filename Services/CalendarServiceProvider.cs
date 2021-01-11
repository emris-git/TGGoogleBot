using System;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;

namespace TGGoogleBot.Services
{
    public class CalendarServiceProvider
    {
        public CalendarService CalendarService { get; set; }
        
        private static readonly string[] Scopes =
        {
            CalendarService.Scope.CalendarReadonly
        };
        private static readonly string ApplicationName = "Google Calendar API .NET Quickstart";

        public CalendarServiceProvider(IConfiguration configuration)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    configuration.GetSection("Google")["UserEmail"],
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            CalendarService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });
        }
    }
}