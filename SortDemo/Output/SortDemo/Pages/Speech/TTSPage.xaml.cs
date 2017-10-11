using Common;
using Common.Model;
using SortDemo.UnityInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
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
    public sealed partial class TTSPage : Page
    {
        MediaPlayer _mediaPlayer = new MediaPlayer();
        TextToSpeechHelper _textToSpeechHelper = new TextToSpeechHelper();

        public TTSPage()
        {
            this.InitializeComponent();
            Voices.ItemsSource = Voice.GetVoices();
            Voices.SelectedIndex = 4;
        }

        private async void Say_Click(object sender, RoutedEventArgs e)
        {
            //IRandomAccessStream audio = null;

            try
            {
                Thinking.Visibility = Visibility.Visible;
                var audio = await _textToSpeechHelper.Convert(TextToSay.Text);

                UnityHelper unityHelper = new UnityHelper();
                unityHelper.PlayAudio(audio);
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

            //if (audio != null)
            //{
            //    _mediaPlayer.Source = MediaSource.CreateFromStream(audio, "audio/wav");
            //    _mediaPlayer.Play();
            //}
        }

        private void Voices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var voice = Voices.SelectedItem as Voice;

            if (voice == null) return;

            _textToSpeechHelper.SetVoice(voice);
        }
    }
}
