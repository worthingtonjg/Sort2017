using Common;
using SortDemo.Events;
using SortDemo.Pages.Vision;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SortDemo.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VisionPage : Page
    {
        public VisionPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var page = e.Parameter as Type;
            if(page == typeof(TakePhotoPage))
            {
                NavigationFrame.Navigate(typeof(TakePhotoPage));
            }
            else if(page == typeof(FaceIdentificationPage))
            {
                NavigationFrame.Navigate(typeof(FaceIdentificationPage), true);
            }
            else if (page == typeof(AnalyzeImagePage))
            {
                NavigationFrame.Navigate(typeof(AnalyzeImagePage), true);
            }
            else if (page == typeof(CelebrityRecognitionPage))
            {
                NavigationFrame.Navigate(typeof(CelebrityRecognitionPage), true);
            }
            else if (page == typeof(ProphetRecognitionPage))
            {
                NavigationFrame.Navigate(typeof(ProphetRecognitionPage), true);
            }
            else if (page == typeof(FaceRecognitionPage))
            {
                NavigationFrame.Navigate(typeof(FaceRecognitionPage), true);
            }
            else
            {
                NavigationFrame.Navigate(typeof(AnalyzeImagePage));
            }
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void ImageAnalyzer_Click(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(typeof(AnalyzeImagePage));
        }

        private void FaceAnalyzer_Click(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(typeof(FaceRecognitionPage));
        }

        private void CelebrityIdentifier_Click(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(typeof(CelebrityRecognitionPage));
        }

        private void ProphetIdentifier_Click(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(typeof(ProphetRecognitionPage));
        }

        private void PersonIdentifier_Click(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(typeof(FaceIdentificationPage));
        }

        private void TakePhoto_Click(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(typeof(TakePhotoPage));
        }
    }
}
