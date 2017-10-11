using Common;
using Common.Model;
using SortDemo.UnityInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SortDemo.Pages.Speech
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Final : Page
    {
        MediaCapture _captureMedia = new MediaCapture();
        InMemoryRandomAccessStream _audioStream = new InMemoryRandomAccessStream();

        public Final()
        {
            this.InitializeComponent();
        }

        private async void Image_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            await StartRecording();
        }

        private async void Image_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            await StopRecordingAndSend();
        }

        private async Task StartRecording()
        {
            _audioStream = new InMemoryRandomAccessStream();
            _captureMedia = new MediaCapture();
            var captureInitSettings = new MediaCaptureInitializationSettings();
            captureInitSettings.StreamingCaptureMode = StreamingCaptureMode.Audio;
            await _captureMedia.InitializeAsync(captureInitSettings);

            MediaEncodingProfile encodingProfile = MediaEncodingProfile.CreateWav(AudioEncodingQuality.Medium);

            await _captureMedia.StartRecordToStreamAsync(encodingProfile, _audioStream);

            MicOff.Visibility = Visibility.Visible;
            MicOn.Visibility = Visibility.Collapsed;
        }

        private async Task StopRecordingAndSend()
        {
            await _captureMedia.StopRecordAsync();

            MicOff.Visibility = Visibility.Collapsed;
            MicOn.Visibility = Visibility.Visible;

            SpeechToTextHelper _speechToTextHelper = new SpeechToTextHelper();

            try
            {
                Thinking.Visibility = Visibility.Visible;

                byte[] bytes = await _speechToTextHelper.GetBytes(_audioStream);
                SpeechToTextResponse question = await _speechToTextHelper.Convert(bytes);

                Question.Text = question.DisplayText;

                var botHelper = new BotHelper(EnumBot.ArticlesOfFaith);
                BotResponse response = await botHelper.Ask(question.DisplayText);

                if (response.answers.Count > 0)
                {
                    Answer.Text = response.answers[0].answer;

                    TextToSpeechHelper textToSpeechHelper = new TextToSpeechHelper();
                    var audioBytes = await textToSpeechHelper.Convert(response.answers[0].answer);
                    
                    UnityHelper unityHelper = new UnityHelper();
                    unityHelper.PlayAudio(audioBytes);
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
    }
}
