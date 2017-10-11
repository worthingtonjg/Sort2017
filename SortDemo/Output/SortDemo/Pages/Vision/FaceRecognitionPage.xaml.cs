using Caliburn.Micro;
using Common;
using Microsoft.ProjectOxford.Face.Contract;
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
using Windows.UI.Input;
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
    public sealed partial class FaceRecognitionPage : Page,
        IHandle<ImageFromClipboardAction>,
        IHandle<AnalyzeImageAction>
    {
        private FaceHelper _faceHelper = new FaceHelper();
        private StorageFile _image;
        private Face[] _faces = null;
        private Dictionary<string, List<Face>> _facesDictionary = new Dictionary<string, List<Face>>();

        public FaceRecognitionPage()
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
                ImageDisplay.Source = null;
                _faces = null;
                FacesCombo.ItemsSource = null;

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

                _faces = await _faceHelper.Detect(_image);
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

            PopulateFaceDictionary();

            string message = DescribeImage(_faces);
            EventBus.Instance.PublishOnUIThread(new SayMessage(message));
        }

        private string DescribeImage(Face[] faces)
        {
            if (faces == null || faces.Length == 0)
            {
                return "I do not see any faces.";
            }

            string message = string.Empty;

            if (faces.Length > 0)
            {
                if (faces.Length == 1)
                {
                    string genderPronoun = "";
                    if (faces[0].FaceAttributes.Gender.ToLower() == "male")
                    {
                        message += $"I see 1 male";
                        genderPronoun = "He";
                    }
                    else
                    {
                        message += $"I see 1 female";
                        genderPronoun = "She";
                    }

                    message += $", age {Math.Truncate(faces[0].FaceAttributes.Age)}.  ";

                    var emotionDictionary = new Dictionary<string, float>();

                    emotionDictionary["angry"] = faces[0].FaceAttributes.Emotion.Anger;
                    emotionDictionary["contemptful"] = faces[0].FaceAttributes.Emotion.Contempt;
                    emotionDictionary["fearful"] = faces[0].FaceAttributes.Emotion.Fear;
                    emotionDictionary["happy"] = faces[0].FaceAttributes.Emotion.Happiness;
                    emotionDictionary["sad"] = faces[0].FaceAttributes.Emotion.Sadness;
                    emotionDictionary["surprised"] = faces[0].FaceAttributes.Emotion.Surprise;
                    emotionDictionary["disgusted"] = faces[0].FaceAttributes.Emotion.Disgust;

                    float mostFeltEmotion = emotionDictionary.Max(e => e.Value);

                    var mostFelt = emotionDictionary.Where(e => e.Value == mostFeltEmotion).Select(e => e.Key).FirstOrDefault();
                    if(emotionDictionary[mostFelt] >= 0.1f)
                    {


                        message += $"{genderPronoun} looks ... {mostFelt}."; 
                    }
                }
                else
                {
                    message += $"I see {faces.Length} faces.  ";
                    foreach(var face in faces)
                    {
                        message += $"A {face.FaceAttributes.Gender}, age {Math.Truncate(face.FaceAttributes.Age)}.  ";
                    }

                    var emotions = new List<string>();
                    if (_facesDictionary.ContainsKey("Most Angry")) emotions.Add("angry");
                    if (_facesDictionary.ContainsKey("Most Contempt")) emotions.Add("contemptful");
                    if (_facesDictionary.ContainsKey("Most Fear")) emotions.Add("fearful");
                    if (_facesDictionary.ContainsKey("Most Happy")) emotions.Add("happy");
                    if (_facesDictionary.ContainsKey("Most Sad")) emotions.Add("sad");
                    if (_facesDictionary.ContainsKey("Most Surprised")) emotions.Add("surprised");
                    if (_facesDictionary.ContainsKey("Most Disgust")) emotions.Add("disgusted");

                    if(emotions.Count > 0)
                    {
                        string emotionsString = String.Join(", and ", emotions);
                        message += $"They look: {emotionsString}.  ";
                    }
                }
            }

            return message;
        }
    

        private void PopulateFaceDictionary()
        {
            if(_faces == null || _faces.Length == 0)
            {
                return;
            }

            // Populate face dictionary
            _facesDictionary.Clear();
            _facesDictionary[string.Format("Found {0} faces", _faces.Length)] = _faces.ToList();
            int i = 1;
            foreach (var face in _faces)
            {
                _facesDictionary[$"Face {i} - {face.FaceAttributes.Gender} {face.FaceAttributes.Age}"] = new List<Face> { face };
                i++;
            }

            // Females
            var females = _faces.Where(f => f.FaceAttributes.Gender == "female").ToList();
            if (females.Count > 0)
            {
                _facesDictionary["Female"] = females;
            }

            // Males
            var males = _faces.Where(f => f.FaceAttributes.Gender == "male").ToList();
            if(males.Count > 0)
            {
                _facesDictionary["Males"] = males;
            }

            // Oldest
            double oldestAge = _faces.Max(f => f.FaceAttributes.Age);
            _facesDictionary["Oldest"] = _faces.Where(f => f.FaceAttributes.Age == oldestAge).ToList();

            // Youngest
            double youngestAge = _faces.Min(f => f.FaceAttributes.Age);
            _facesDictionary["Youngest"] = _faces.Where(f => f.FaceAttributes.Age == youngestAge).ToList();


            // Most Angry
            var angry = _faces.Where(f => f.FaceAttributes.Emotion.Anger >= 0.1f).ToList();
            if(angry.Count > 0)
            {
                float mostAngry = angry.Max(f => f.FaceAttributes.Emotion.Anger);
                _facesDictionary["Most Angry"] = _faces.Where(f => f.FaceAttributes.Emotion.Anger == mostAngry).ToList();
            }

            // Most Contempt
            var contempt = _faces.Where(f => f.FaceAttributes.Emotion.Contempt >= 0.1f).ToList();
            if (contempt.Count > 0)
            {
                float mostContempt = contempt.Max(f => f.FaceAttributes.Emotion.Contempt);
                _facesDictionary["Most Contempt"] = _faces.Where(f => f.FaceAttributes.Emotion.Contempt == mostContempt).ToList();
            }

            // Most Disgust
            var disgust = _faces.Where(f => f.FaceAttributes.Emotion.Disgust >= 0.1f).ToList();
            if (disgust.Count > 0)
            {
                float mostDisgust = disgust.Max(f => f.FaceAttributes.Emotion.Disgust);
                _facesDictionary["Most Disgust"] = _faces.Where(f => f.FaceAttributes.Emotion.Disgust == mostDisgust).ToList();
            }

            // Most Fear
            var fear = _faces.Where(f => f.FaceAttributes.Emotion.Fear >= 0.1f).ToList();
            if (fear.Count > 0)
            {
                float mostFear = fear.Max(f => f.FaceAttributes.Emotion.Fear);
                _facesDictionary["Most Fear"] = _faces.Where(f => f.FaceAttributes.Emotion.Fear == mostFear).ToList();
            }

            // Most Happy
            var happy = _faces.Where(f => f.FaceAttributes.Emotion.Happiness >= 0.1f).ToList();
            if (happy.Count > 0)
            {
                float mostHappy = happy.Max(f => f.FaceAttributes.Emotion.Happiness);
                _facesDictionary["Most Happy"] = _faces.Where(f => f.FaceAttributes.Emotion.Happiness == mostHappy).ToList();
            }

            // Most Sad
            var sad = _faces.Where(f => f.FaceAttributes.Emotion.Sadness >= 0.1f).ToList();
            if (sad.Count > 0)
            {
                float mostSad = sad.Max(f => f.FaceAttributes.Emotion.Sadness);
                _facesDictionary["Most Sad"] = _faces.Where(f => f.FaceAttributes.Emotion.Sadness == mostSad).ToList();
            }

            // Most Surpised
            var surprised = _faces.Where(f => f.FaceAttributes.Emotion.Surprise >= 0.1f).ToList();
            if (surprised.Count > 0)
            {
                float mostSurprised = surprised.Max(f => f.FaceAttributes.Emotion.Surprise);
                _facesDictionary["Most Surpised"] = _faces.Where(f => f.FaceAttributes.Emotion.Surprise == mostSurprised).ToList();
            }

            FacesCombo.ItemsSource = _facesDictionary;
            FacesCombo.SelectedIndex = 0;
        }

        private void FacesCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_faces == null || _faces.Length == 0) return;

            var faces = FacesCombo.SelectedValue as List<Face>;

            UpdateFaces(faces);
            Results.Text = JsonConvert.SerializeObject(faces, Formatting.Indented);
        }

        private async void UpdateFaces(List<Face> selectedFaces)
        {
            var imageSource = await _faceHelper.MarkFaces(_image, selectedFaces.ToArray());

            if (imageSource != null)
            {
                // Display Image
                ImageDisplay.Source = imageSource;

                // Set the zoom level
                var uiHelper = new UIHelper();
                uiHelper.SetZoom(ImageZoom, ImageDisplay);
            }
        }

        private async void ImageFromClipboard_Click(object sender, RoutedEventArgs e)
        {
            await ImageFromClipboard(false);
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
    }
}
