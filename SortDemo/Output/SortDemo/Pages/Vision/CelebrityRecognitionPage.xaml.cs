using Caliburn.Micro;
using Common;
using Common.Model;
using Microsoft.ProjectOxford.Vision.Contract;
using Newtonsoft.Json;
using SortDemo.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SortDemo.Pages.Vision
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CelebrityRecognitionPage : Page, 
        IHandle<ImageFromClipboardAction>,
        IHandle<AnalyzeImageAction>
    {
        private StorageFile _image;
        private VisionHelper _visionHelper = new VisionHelper();
        private Dictionary<string, List<Celebrity>> _facesDictionary = new Dictionary<string, List<Celebrity>>();

        public CelebrityRecognitionPage()
        {
            this.InitializeComponent();
            EventBus.Instance.Subscribe(this);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null)
            {
                bool autoIdentify = (bool)e.Parameter;

                if (autoIdentify)
                {
                    await ImageFromClipboard(false);
                    await AnalyzeImage();
                }
            }
        }

        private async Task ImageFromClipboard(bool vocal = true)
        {
            try
            {
                Thinking.Visibility = Visibility.Visible;

                Results.Text = string.Empty;
                Celebrities.ItemsSource = null;

                var _clipboardHelper = new ClipboardHelper();

                // Get image from clipboard and store in picture library
                _image = await _clipboardHelper.ImageFromClipboard();

                // Now get the image
                BitmapImage imageSource = await _clipboardHelper.ImageSourceFromStorageFile(_image);

                // Display the image the image
                ImageDisplay.Source = imageSource;

                // Set the zoom level
                var uiHelper = new UIHelper();
                uiHelper.SetZoom(ImageZoom, ImageDisplay);

                if (vocal)
                {
                    EventBus.Instance.PublishOnUIThread(new SayMessage(BotConstants.HereItIs));
                }
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }
            finally
            {
                Thinking.Visibility = Visibility.Collapsed;
            }
        }

        private async Task AnalyzeImage()
        {
            if (_image == null)
            {
                return;
            }

            try
            {
                Thinking.Visibility = Visibility.Visible;

                List<Celebrity> celebrities = await _visionHelper.IdentifyCelebrities(_image);

                Results.Text = JsonConvert.SerializeObject(celebrities, Formatting.Indented);

                PopulateCelebrities(celebrities);

                string message = DescribeImage(celebrities);
                EventBus.Instance.PublishOnUIThread(new SayMessage(message));
                
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }
            finally
            {
                Thinking.Visibility = Visibility.Collapsed;
            }
        }

        private string DescribeImage(List<Celebrity> celebrities)
        {
            string message = string.Empty;

            if(celebrities.Count == 0)
            {
                return "I can not identify any celebrities";
            }

            string celebrityWord = "celebrity";
            if(celebrities.Count > 1)
            {
                celebrityWord = "celebrities";
            }

            message = $"I found {celebrities.Count} {celebrityWord} ... ";

            foreach(var celebrity in celebrities)
            {
                message += $"{celebrity.Name}, ";
            }

            return message;

        }

        private void PopulateCelebrities(List<Celebrity> celebrities)
        {
            string allFaces = string.Format("Found {0} celebrities", celebrities.Count);
            _facesDictionary.Clear();
            _facesDictionary[allFaces] = celebrities;

            int i = 1;
            foreach (var c in celebrities)
            {
                _facesDictionary[$"{c.Name} {Math.Truncate(c.Confidence * 100)}%"] = new List<Celebrity> { c };
                i++;
            }

            Celebrities.ItemsSource = _facesDictionary;
            Celebrities.SelectedIndex = 0;
        }

        private async void ImageFromClipboard_Click(object sender, RoutedEventArgs e)
        {
            await ImageFromClipboard(vocal: false);
        }

        private async void AnalyzeImage_Click(object sender, RoutedEventArgs e)
        {
            await AnalyzeImage();
        }

        public async void Handle(ImageFromClipboardAction message)
        {
            await ImageFromClipboard();
        }

        public async void Handle(AnalyzeImageAction message)
        {
            await AnalyzeImage();
        }

        private async void Celebrities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Celebrities.Items == null || Celebrities.Items.Count == 0) return;

            var celebrities = Celebrities.SelectedValue as List<Celebrity>;
            var faces = celebrities.Select(c => new Face { FaceRectangle = c.FaceRectangle }).ToList();

            await UpdateFaces(faces);
        }

        private async Task UpdateFaces(List<Face> selectedFaces)
        {
            var imageSource = await _visionHelper.MarkFaces(_image, selectedFaces.ToArray());

            if (imageSource != null)
            {
                ImageDisplay.Source = imageSource;
            }
        }
    }
}
