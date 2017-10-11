using Common.Model;
using Microsoft.ProjectOxford.Vision.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace Common
{
    public class VisionHelper
    {
        private string _subscriptionKey = "c2d487d47ddc49e2b08738e16f37d8fa";
        private string _uriBase = "https://westus.api.cognitive.microsoft.com/vision/v1.0/analyze";

        public async Task<AnalysisResult> Analyze(StorageFile file)
        {
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);

            // Request parameters. A third optional parameter is "details".
            string requestParameters = "visualFeatures=Categories,Tags,Description,ImageType,Color,Adult&details=Landmarks&language=en";

            // Assemble the URI for the REST API Call.
            string uri = _uriBase + "?" + requestParameters;

            HttpResponseMessage response;

            // Request body. Posts a locally stored JPEG image.
            byte[] byteData = await GetImageAsByteArray(file);

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                response = await client.PostAsync(uri, content);

                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(contentString);

                var result = JsonConvert.DeserializeObject<AnalysisResult>(contentString);

                result = await FindLandmark(result);
                
                return result;
            }
        }

        public async Task<List<Celebrity>> IdentifyCelebrities(StorageFile file)
        {
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);

            // Request parameters. A third optional parameter is "details".
            string requestParameters = "details=Celebrities&language=en";

            // Assemble the URI for the REST API Call.
            string uri = _uriBase + "?" + requestParameters;

            HttpResponseMessage response;

            // Request body. Posts a locally stored JPEG image.
            byte[] byteData = await GetImageAsByteArray(file);

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                response = await client.PostAsync(uri, content);

                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(contentString);

                var analysisResult = JsonConvert.DeserializeObject<AnalysisResult>(contentString);

                var result = await FindCelebrities(analysisResult);

                return result;
            }
        }

        public async Task<WriteableBitmap> MarkFaces(StorageFile file, Face[] faces)
        {
            if (faces.Length == 0) return null;

            using (var stream = await file.OpenStreamForReadAsync())
            {
                WriteableBitmap wb = await BitmapFactory.FromStream(stream);
                using (wb.GetBitmapContext())
                {

                    for (int i = 0; i < faces.Length; ++i)
                    {
                        Face face = faces[i];

                        wb.DrawRectangle(
                            face.FaceRectangle.Left,
                            face.FaceRectangle.Top,
                            face.FaceRectangle.Left + face.FaceRectangle.Width,
                            face.FaceRectangle.Top + face.FaceRectangle.Height,
                            Colors.Red
                            );
                    }
                }

                return wb;
            }
        }

        private async Task<AnalysisResult> FindLandmark(AnalysisResult result)
        {
            if (result == null || result.Categories == null) return result;

            await Task.Run(() =>
            {
                string landmark = string.Empty;

                var landmarks = new List<Caption>();
                foreach (var category in result.Categories)
                {
                    if (category.Detail != null)
                    {
                        try
                        {
                            var categoryDetail = JsonConvert.DeserializeObject<CategoryDetail>(category.Detail.ToString());
                            if (categoryDetail != null)
                            {
                                landmarks.AddRange(categoryDetail.LandsmarksToCaptions());
                            }
                        }
                        catch
                        {
                            // Eat the exception for now
                        }
                    }
                }

                if (landmarks.Count > 0)
                {
                    var mostConfidence = landmarks.OrderBy(l => l.Confidence).FirstOrDefault();

                    List<Caption> captions = result.Description.Captions.ToList();
                    captions.Insert(0, mostConfidence);

                    result.Description.Captions = captions.ToArray();
                }
            });

            return result;
        }

        private async Task<List<Celebrity>> FindCelebrities(AnalysisResult result)
        {
            var celebrities = new List<Celebrity>();
            if (result == null || result.Categories == null) return celebrities;

            await Task.Run(() =>
            {
                foreach (var category in result.Categories)
                {
                    if (category.Detail != null)
                    {
                        try
                        {
                            var categoryDetail = JsonConvert.DeserializeObject<CategoryDetail>(category.Detail.ToString());
                            if (categoryDetail != null)
                            {
                                celebrities.AddRange(categoryDetail.Celebrities);
                            }
                        }
                        catch
                        {
                            // Eat the exception for now
                        }
                    }
                }
            });

            return celebrities;
        }

        private async Task<byte[]> GetImageAsByteArray(StorageFile file)
        {
            byte[] result = null;

            using (var stream = await file.OpenStreamForReadAsync())
            {
                result = new byte[(int)stream.Length];
                stream.Read(result, 0, (int)stream.Length);
            }

            return result;
        }
    }
}
