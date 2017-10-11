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
    public sealed partial class AnalyzeImagePage : Page, 
        IHandle<ImageFromClipboardAction>,
        IHandle<AnalyzeImageAction>
    {
        private StorageFile _image;
        private ClipboardHelper _clipboardHelper = new ClipboardHelper();

        public AnalyzeImagePage()
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
                Caption.Text = string.Empty;

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

                var _visionHelper = new VisionHelper();

                AnalysisResult result = await _visionHelper.Analyze(_image);

                if (result != null && result.Description != null && result.Description.Captions != null && result.Description.Captions.Length > 0)
                {
                    Caption.Text = result.Description.Captions.FirstOrDefault().Text;

                    Results.Text = JsonConvert.SerializeObject(result, Formatting.Indented);

                    EventBus.Instance.PublishOnUIThread(new SayMessage("I see ..." + Caption.Text));
                }
                else
                {
                    throw new Exception("Error analyzing image - no caption returned");
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

        
    }
}
