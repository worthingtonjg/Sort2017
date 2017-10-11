using Common.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum EnumBot
    {
        MormonNewsRoomFaq,
        ArticlesOfFaith,
        NavigationAI
    }

    public class BotHelper
    {
        private EnumBot _bot;
        private Dictionary<EnumBot, Bot> _bots;
        private Bot _currentBot;

        public BotHelper(EnumBot bot)
        {
            InitBots();

            _bot = bot;
            _currentBot = _bots[bot];
        }

        private void InitBots()
        {
            _bots = new Dictionary<EnumBot, Bot>();
            _bots[EnumBot.MormonNewsRoomFaq] = new Bot("2bb659be5cd54ef9b7455581a117a907", @"https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases/5649976f-c288-4ab0-ba1c-2853440c459a/generateAnswer");
            _bots[EnumBot.ArticlesOfFaith] = new Bot("2bb659be5cd54ef9b7455581a117a907", @"https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases/9a6ca607-6c84-415d-bbda-fbe633555131/generateAnswer");
            _bots[EnumBot.NavigationAI] = new Bot("2bb659be5cd54ef9b7455581a117a907", @"https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases/0e6855a2-6c2b-465b-ba36-c6fcd61df79e/generateAnswer");
        }

        public void SetBot(EnumBot bot)
        {
            _bot = bot;
            _currentBot = _bots[bot];
        }

        public async Task<BotResponse> Ask(string question)
        {
            BotResponse result = null;

            var client = new HttpClient();

            client.DefaultRequestHeaders.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", _currentBot.Key);

            var request = new HttpRequestMessage(HttpMethod.Post, _currentBot.Uri)
            {
                Content = new StringContent("{\"question\":\"" + question + "\"}")
            };

            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await client.SendAsync(request);

            Console.WriteLine("Response status code: [{0}]", response.StatusCode);

            try
            {
                if (response.IsSuccessStatusCode)
                {

                    string json = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(json);

                    result = JsonConvert.DeserializeObject<BotResponse>(json);

                    foreach(var answer in result.answers)
                    {
                        answer.answer = WebUtility.HtmlDecode(answer.answer);
                    }
                }
                else
                {
                    throw new Exception(String.Format("Service returned {0}", response.StatusCode));
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                response.Dispose();
                request.Dispose();
                client.Dispose();
            }


            return result;
        }
    }
}
