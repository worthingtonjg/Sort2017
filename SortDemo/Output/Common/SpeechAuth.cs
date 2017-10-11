using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public class SpeechAuth
    {
        private string subscriptionKey = "5af27030917047e88ec881c84b253134";
        private string token;
        private Timer accessTokenRenewer;
        private const int RefreshTokenDuration = 9;

        private static SpeechAuth _instance;

        public static async Task<SpeechAuth> GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SpeechAuth();
                await _instance.Init();
            }

            return _instance;
        }

        public async Task Init()
        {
            token = await FetchToken();

            // renew the token on set duration.
            accessTokenRenewer = new Timer(
                new TimerCallback(OnTokenExpiredCallback),
                this,
                TimeSpan.FromMinutes(RefreshTokenDuration),
                TimeSpan.FromMilliseconds(-1));
        }

        public string GetAccessToken()
        {
            return this.token;
        }

        private async Task RenewAccessToken()
        {
            token = await FetchToken();
            Console.WriteLine("Renewed token.");
        }

        private async void OnTokenExpiredCallback(object stateInfo)
        {
            try
            {
                await RenewAccessToken();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Failed renewing access token. Details: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    accessTokenRenewer.Change(TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Failed to reschedule the timer to renew access token. Details: {0}", ex.Message));
                }
            }
        }

        private async Task<string> FetchToken()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                UriBuilder uriBuilder = new UriBuilder(@"https://api.cognitive.microsoft.com/sts/v1.0");
                uriBuilder.Path += "/issueToken";

                var result = await client.PostAsync(uriBuilder.Uri.AbsoluteUri, null);
                Console.WriteLine("Token Uri: {0}", uriBuilder.Uri.AbsoluteUri);
                return await result.Content.ReadAsStringAsync();
            }
        }
    }
}
