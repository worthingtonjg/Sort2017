using Common.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Common
{
    public class TextToSpeechHelper
    {
        private const string SsmlTemplate = "<speak version='1.0' xml:lang='en-us'><voice xml:lang='{0}' xml:gender='{1}' name='{2}'>{3}</voice></speak>";

        TextToSpeechInput inputOptions;

        public Action<Stream> OnAudioAvailable;

        public Action<Exception> OnError;

        public TextToSpeechHelper(TextToSpeechInput input = null)
        {
            if (input != null)
            {
                inputOptions = input;
            }
            else
            {
                inputOptions = new TextToSpeechInput();
            }
        }

        public void SetVoice(Voice voice)
        {
            inputOptions.Voice = voice;
        }

        public async Task<byte[]> Convert(string text)
        {
            var client = new HttpClient();

            var headers = await inputOptions.GetHeaders();

            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
            }

            var genderValue = this.inputOptions.Voice.Gender.ToString();

            var request = new HttpRequestMessage(HttpMethod.Post, @"https://speech.platform.bing.com/synthesize")
            {
                Content = new StringContent(String.Format(SsmlTemplate, this.inputOptions.Voice.Locale, genderValue, this.inputOptions.Voice.ServiceName, text))
            };

            HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            Console.WriteLine("Response status code: [{0}]", response.StatusCode);

            try
            {
                if (response.IsSuccessStatusCode)
                {
                    var bytes = await response.Content.ReadAsByteArrayAsync();

                    return bytes;
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
        }


    }
}
