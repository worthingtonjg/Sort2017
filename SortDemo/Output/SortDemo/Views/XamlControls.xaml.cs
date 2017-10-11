using Caliburn.Micro;
using Common;
using SortDemo.Events;
using SortDemo.Pages;
using SortDemo.Pages.Vision;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SortDemo.Views
{
    public sealed partial class XamlControls : UserControl, 
        IHandle<GotoTakePhotoPageAction>, 
        IHandle<GotoPersonIdentifierPageAction>,
        IHandle<GotoAnalyzeImagePageAction>,
        IHandle<GotoCelebrityIdentifierPageAction>,
        IHandle<GotoChurchLeaderIdentifierPageAction>,
        IHandle<GotoFaceRecognitionPageAction>
    {
        public XamlControls()
        {
            this.InitializeComponent();
            EventBus.Instance.Subscribe(this);

            NavigationFrame.Navigate(typeof(SpeechPage));
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void VisionButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(typeof(VisionPage));
        }

        private void Speech_Click(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(typeof(SpeechPage));
        }

        private void Language_Click(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(typeof(LanguagePage));
        }

        private void Knowledge_Click(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(typeof(KnowledgePage));
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(typeof(SearchPage));
        }

        public void Handle(GotoTakePhotoPageAction message)
        {
            NavigationFrame.Navigate(typeof(VisionPage), typeof(TakePhotoPage));
        }

        public void Handle(GotoPersonIdentifierPageAction message)
        {
            NavigationFrame.Navigate(typeof(VisionPage), typeof(FaceIdentificationPage));
        }

        public void Handle(GotoAnalyzeImagePageAction message)
        {
            NavigationFrame.Navigate(typeof(VisionPage), typeof(AnalyzeImagePage));
        }

        public void Handle(GotoCelebrityIdentifierPageAction message)
        {
            NavigationFrame.Navigate(typeof(VisionPage), typeof(CelebrityRecognitionPage));
        }

        public void Handle(GotoChurchLeaderIdentifierPageAction message)
        {
            NavigationFrame.Navigate(typeof(VisionPage), typeof(ProphetRecognitionPage));
        }

        public void Handle(GotoFaceRecognitionPageAction message)
        {
            NavigationFrame.Navigate(typeof(VisionPage), typeof(FaceRecognitionPage));
        }
    }
}
