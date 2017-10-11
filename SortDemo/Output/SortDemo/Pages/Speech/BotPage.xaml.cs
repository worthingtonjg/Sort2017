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
    public sealed partial class BotPage : Page
    {
        MediaCapture _captureMedia = new MediaCapture();
        InMemoryRandomAccessStream _audioStream = new InMemoryRandomAccessStream();
        BotHelper _botHelper = new BotHelper(EnumBot.ArticlesOfFaith);

        public BotPage()
        {
            this.InitializeComponent();
            BotsCombo.SelectedIndex = 0;
        }
        
        private void BotsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _botHelper.SetBot((EnumBot)BotsCombo.SelectedIndex);
        }

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thinking.Visibility = Visibility.Visible;

                BotResponse response = await _botHelper.Ask(Question.Text);

                if (response.answers.Count > 0)
                { 
                    Answer.Text = response.answers[0].answer;
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
