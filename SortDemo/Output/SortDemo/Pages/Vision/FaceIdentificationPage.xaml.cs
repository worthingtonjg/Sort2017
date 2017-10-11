using Common;
using Common.Model;
using Microsoft.ProjectOxford.Face.Contract;
using Newtonsoft.Json;
using SortDemo.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using Caliburn.Micro;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SortDemo.Pages.Vision
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FaceIdentificationPage : Page,
        IHandle<ImageFromClipboardAction>,
        IHandle<AnalyzeImageAction>
    {
        private StorageFile _image;
        private FaceHelper _faceHelper = new FaceHelper();
        private string _personGroupId = "myfriends";
        private Dictionary<string, List<Identification>> _facesDictionary = new Dictionary<string, List<Identification>>();

        public FaceIdentificationPage()
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
                FacesCombo.ItemsSource = null;

                var _clipboardHelper = new ClipboardHelper();

                // Get image from clipboard and store in picture library
                _image = await _clipboardHelper.ImageFromClipboard();

                // Now get the image
                BitmapImage imageSource = await _clipboardHelper.ImageSourceFromStorageFile(_image);
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
                List<Identification> people = await _faceHelper.Identify(_personGroupId, _image);

                PopulateFaces(people);

                string message = _faceHelper.DescribeImage(people);
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

        private async void ImageFromClipboard_Click(object sender, RoutedEventArgs e)
        {
            await ImageFromClipboard(vocal:false);
        }

        private async void AnalyzeImage_Click(object sender, RoutedEventArgs e)
        {
            await AnalyzeImage();
        }

        private async void AddIdentity_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PersonName.Text)) return;

            try
            {
                CreatePersonResult result = await _faceHelper.AddPerson(_personGroupId, PersonName.Text);
                await _faceHelper.AddImageToPerson(_personGroupId, result.PersonId, _image);
                await _faceHelper.TrainGroup(_personGroupId);
            }
            catch(Exception ex)
            {
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }
            finally
            {
                Thinking.Visibility = Visibility.Collapsed;
            }
        }

        private void PopulateFaces(List<Identification> people)
        {
            if (people == null)
            {
                return;
            }

            string allFaces = string.Format("Found {0} people", people.Count);
            _facesDictionary.Clear();
            _facesDictionary[allFaces] = null;

            int i = 1;
            foreach (var p in people)
            {
                double confidence = 1;
                if (p.IdentifyResult.Candidates.Length > 0)
                {
                    confidence = p.IdentifyResult.Candidates[0].Confidence;
                }

                _facesDictionary[$"{p.Person.Name} [{p.Face.FaceAttributes.Gender} {p.Face.FaceAttributes.Age}] {Math.Truncate(confidence * 100)}%"] = new List<Identification> { p };
                i++;
            }

            _facesDictionary[allFaces] = people;

            FacesCombo.ItemsSource = _facesDictionary;
            FacesCombo.SelectedIndex = 0;
        }

        private async void FacesCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FacesCombo.Items == null || FacesCombo.Items.Count == 0) return;

            var people = FacesCombo.SelectedValue as List<Identification>;

            var faces = people.Select(p => p.Face).ToList();

            await UpdateFaces(faces);
            Results.Text = JsonConvert.SerializeObject(faces, Formatting.Indented);
        }

        private async Task UpdateFaces(List<Face> selectedFaces)
        {
            var imageSource = await _faceHelper.MarkFaces(_image, selectedFaces.ToArray());

            if (imageSource != null)
            {
                ImageDisplay.Source = imageSource;
            }
        }

        public async void Handle(ImageFromClipboardAction message)
        {
            await ImageFromClipboard();
        }

        public async void Handle(AnalyzeImageAction message)
        {
            await AnalyzeImage();
        }
    }
}
