using Caliburn.Micro;
using SortDemo.Events;
using SortDemo.Pages.Speech;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SortDemo.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SpeechPage : Page
    {
        public SpeechPage()
        {
            this.InitializeComponent();
            NavigationFrame.Navigate(typeof(TTSPage));
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void TTS_Click(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(typeof(TTSPage));
        }

        private void STT_Click(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(typeof(STTPage));
        }

        private void BOT_Click(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(typeof(BotPage));
        }

        private void FIN_Click(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(typeof(Final));
        }

        private void ShowVoiceControls_Click(object sender, RoutedEventArgs e)
        {
            EventBus.Instance.PublishOnUIThread(new ActivateVoiceControlsMessage());
        }
    }
}
