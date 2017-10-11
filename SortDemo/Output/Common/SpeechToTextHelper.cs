using Common.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Common
{
    public class SpeechToTextHelper
    {
        public async Task<SpeechToTextResponse> Convert(byte[] audioBytes)
        {
            SpeechToTextResponse result = null;

            var auth = await SpeechAuth.GetInstance();

            var client = new HttpClient();
            
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.GetAccessToken());

            string requestUri = @"https://speech.platform.bing.com/speech/recognition/interactive/cognitiveservices/v1?language=en-US";
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Content = new ByteArrayContent(audioBytes);
            request.Content.Headers.TryAddWithoutValidation("Content-Type", "audio / wav; samplerate = 16000");

            HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            
            try
            {
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    Debug.WriteLine(json);

                    result = JsonConvert.DeserializeObject<SpeechToTextResponse>(json);
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


            /*
            await Task.Run(() => {
                HttpWebRequest request = null;
                string requestUri = @"https://speech.platform.bing.com/speech/recognition/interactive/cognitiveservices/v1?language=en-US";
                request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
                request.SendChunked = true;
                request.Accept = @"application/json;text/xml";
                request.Method = "POST";
                request.ProtocolVersion = HttpVersion.Version11;

                request.ContentType = @"audio/wav; codec=""audio/pcm""; samplerate=16000";
                request.Headers["Authorization"] = "Bearer " + auth.GetAccessToken()

                stream.Position = 0;
                byte[] buffer = null;
                int bytesRead = 0;
                using (Stream requestStream = request.GetRequestStream())
                {
                    
                    buffer = new Byte[checked((uint)Math.Min(1024, (int)stream.Length))];
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        requestStream.Write(buffer, 0, bytesRead);
                    }

                    // Flush
                    requestStream.Flush();
                }

                string responseString = "";
                using (WebResponse response = request.GetResponse())
                {
                    Debug.WriteLine(((HttpWebResponse)response).StatusCode);

                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        responseString = sr.ReadToEnd();
                    }

                    Debug.WriteLine(responseString);

                    result = JsonConvert.DeserializeObject<SpeechToTextResponse>(responseString);

                    
                }
            });
    

            return result;*/
        }

        public async Task<byte[]> GetBytes(IRandomAccessStream _audioStream)
        {
            var reader = new DataReader(_audioStream.GetInputStreamAt(0));
            var bytes = new byte[_audioStream.Size];
            await reader.LoadAsync((uint)_audioStream.Size);
            reader.ReadBytes(bytes);

            return bytes;
        }
    }
}
