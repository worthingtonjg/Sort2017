using Caliburn.Micro;
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
    public sealed partial class ProphetRecognitionPage : Page,
        IHandle<ImageFromClipboardAction>,
        IHandle<AnalyzeImageAction>
    {
        private StorageFile _image;
        private FaceHelper _faceHelper = new FaceHelper();

        private Dictionary<string, List<Identification>> _facesDictionary = new Dictionary<string, List<Identification>>();

        public ProphetRecognitionPage()
        {
            this.InitializeComponent();
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

                EventBus.Instance.PublishOnUIThread(new SayMessage(BotConstants.HereItIs));
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
                List<Identification> people = await _faceHelper.Identify("prophets", _image);

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

        public async void Handle(ImageFromClipboardAction message)
        {
            await ImageFromClipboard();
        }

        public async void Handle(AnalyzeImageAction message)
        {
            await AnalyzeImage();
        }

        private void PopulateFaces(List<Identification> people)
        {
            if (people == null) return;

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

                _facesDictionary[$"{p.Person.Name} [{p.Face.FaceAttributes.Gender} {p.Face.FaceAttributes.Age}] {Math.Truncate(confidence*100)}%"] = new List<Identification> { p };
                i++;
            }

            _facesDictionary[allFaces] = people;

            FacesCombo.ItemsSource = _facesDictionary;
            FacesCombo.SelectedIndex = 0;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var faceHelper = new FaceHelper();
            var photos = new List<Photo>();

            #region photos

            photos.Add(new Photo("Aidukaitis-Marcos-A-large.jpg", "Marcos A. Aidukaitis"));
            photos.Add(new Photo("Alonso-Jose-L-large.jpg", "Jose L. Alonso"));
            photos.Add(new Photo("Andersen-Wilford-W-large.jpg", "Wilford W. Andersen"));
            photos.Add(new Photo("Ardern-Ian-S-large.jpg", "Ian S. Ardern"));
            photos.Add(new Photo("Arnold-Mervyn-B-large.jpg", "Mervyn B. Arnold"));
            photos.Add(new Photo("Bassett-W-Mark-large.jpg", "W. Mark Bassett"));
            photos.Add(new Photo("Baxter-David-S-large.jpg", "David S. Baxter"));
            photos.Add(new Photo("Bennett-Randall-K-large.jpg", "Randall K. Bennett"));
            photos.Add(new Photo("Bowen-Shayne-M-large.jpg", "Shayne M. Bowen"));
            photos.Add(new Photo("Bragg-Mark-A-large.jpg", "Mark A. Bragg"));
            photos.Add(new Photo("Cardon-Craig-A-large.jpg", "Craig A. Cardon"));
            photos.Add(new Photo("Choi-Yoon-Hwan-large.jpg", "Yoon Hwan Choi"));
            photos.Add(new Photo("Clark-Kim-B-large.jpg", "Kim B. Clark"));
            photos.Add(new Photo("Clayton-Weatherford-T-large.jpg", "Weatherford T. Clayton"));
            photos.Add(new Photo("Cook-Carl-B-large.jpg", "Carl B. Cook"));
            photos.Add(new Photo("Corbridge-Lawrence-E-large.jpg", "Lawrence E. Corbridge"));
            photos.Add(new Photo("Cordon-Valeri-V-large.jpg", "Valeri V. Cordón"));
            photos.Add(new Photo("Cornish-J-Devn-large.jpg", "J. Devn Cornish"));
            photos.Add(new Photo("Costa-Claudio-R-M-small.jpg", "Claudio R. M. Costa"));
            photos.Add(new Photo("Costa-Joaquin-E-large.jpg", "Joaquin E. Costa"));
            photos.Add(new Photo("Curtis-JR-LeGrand-R-large.jpg", "LeGrand R. Curtis Jr."));
            photos.Add(new Photo("De-Feo-Massimo-large.jpg", "Massimo De Feo"));
            photos.Add(new Photo("De-Hoyos-Benjamin-large.jpg", "Benjamin De Hoyos"));
            photos.Add(new Photo("Dube-Edward-large.jpg", "Edward Dube"));
            photos.Add(new Photo("Duncan-Kevin-R-large.jpg", "Kevin R. Duncan"));
            photos.Add(new Photo("Dyches-Timothy-J-large.jpg", "Timothy J. Dyches"));
            photos.Add(new Photo("Echo-Hawk-Larry-J-large.jpg", "Larry J. Echo Hawk"));
            photos.Add(new Photo("Ellis-Stanley-G-large.jpg", "Stanley G. Ellis"));
            photos.Add(new Photo("Evans-David-F-large.jpg", "David F. Evans"));
            photos.Add(new Photo("Falabella-Enrique-R-large.jpg", "Enrique R. Falabella"));
            photos.Add(new Photo("Foster-Bradley-D-large.jpg", "Bradley D. Foster"));
            photos.Add(new Photo("Funk-Randy-D-large.jpg", "Randy D. Funk"));
            photos.Add(new Photo("Gavarett-Eduardo-large.jpg", "Eduardo Gavarret"));
            photos.Add(new Photo("Gay-Robert-C-large.jpg", "Robert C. Gay"));
            photos.Add(new Photo("Godoy-Carlos-A-large.jpg", "Carlos A. Godoy"));
            photos.Add(new Photo("Godoy-Taylor-G-large.jpg", "Taylor G. Godoy"));
            photos.Add(new Photo("Golden-Christoffel-large.jpg", "Christoffel Golden"));
            photos.Add(new Photo("Gonzalez-Walter-F-large.jpg", "Walter F. González"));
            photos.Add(new Photo("Grow-C-Scott-large.jpg", "C. Scott Grow"));
            photos.Add(new Photo("Haleck-O-Vincent-large.jpg", "O. Vincent Haleck"));
            photos.Add(new Photo("Hallstrom-Donald-L-large.jpg", "Donald L. Hallstrom"));
            photos.Add(new Photo("Hamilton-Kevin-S-large.jpg", "Kevin S. Hamilton"));
            photos.Add(new Photo("Haynie-Allen-D-large.jpg", "Allen D. Haynie"));
            photos.Add(new Photo("Johnson-Paul-V-large.jpg", "Paul V. Johnson"));
            photos.Add(new Photo("Kacher-Larry-S-large.jpg", "Larry S. Kacher"));
            photos.Add(new Photo("Keetch-Von-G-large.jpg", "Von G. Keetch"));
            photos.Add(new Photo("Klebingat-Jorg-large.jpg", "Jörg Klebingat"));
            photos.Add(new Photo("Koch-Joni-L-large.jpg", "Joni L. Koch"));
            photos.Add(new Photo("Kopischke-Erich-W-large.jpg", "Erich W. Kopischke"));
            photos.Add(new Photo("Lawrence-Larry-R-large.jpg", "Larry R. Lawrence"));
            photos.Add(new Photo("Martinez-Hugo-E-large.jpg", "Hugo E. Martinez"));
            photos.Add(new Photo("Martino-James-B-large.jpg", "James B. Martino"));
            photos.Add(new Photo("Maynes-Richard-J-large.jpg", "Richard J. Maynes"));
            photos.Add(new Photo("Meurs-Peter-F-large.jpg", "Peter F. Meurs"));
            photos.Add(new Photo("Montoya-Hugo-large.jpg", "Hugo Montoya"));
            photos.Add(new Photo("Nash-Marcus-B-large.jpg", "Marcus B. Nash"));
            photos.Add(new Photo("Nattress-Brett-K-large.jpg", "K. Brett Nattress"));
            photos.Add(new Photo("Nielsen-S-Gifford-large.jpg", "S. Gifford Nielsen"));
            photos.Add(new Photo("Nielson-Brent-H-large.jpg", "Brent H. Nielson"));
            photos.Add(new Photo("Ochoa-Adrian-large.jpg", "Adrián Ochoa"));
            photos.Add(new Photo("Packer-Allan-F-large.jpg", "Allan F. Packer"));
            photos.Add(new Photo("Palmer-S-Mark-large.jpg", "S. Mark Palmer"));
            photos.Add(new Photo("Parrella-Adilsonde-Paula-large.jpg", "Adilson de Paula Parrella"));
            photos.Add(new Photo("Pearson-Kevin-W-large.jpg", "Kevin W. Pearson"));
            photos.Add(new Photo("Perkins-Anthony-D-large.jpg", "Anthony D. Perkins"));
            photos.Add(new Photo("Pieper-Paul-B-large.jpg", "Paul B. Pieper"));
            photos.Add(new Photo("Pingree-John-C-Jr-large.jpg", "John C. Pingree Jr."));
            photos.Add(new Photo("Pino-Rafael-E-large.jpg", "Rafael E. Pino"));
            photos.Add(new Photo("Ringwood-Michael-T-large.jpg", "Michael T. Ringwood"));
            photos.Add(new Photo("Sabin-Gary-B-large.jpg", "Gary B. Sabin"));
            photos.Add(new Photo("Schmutz-Evan-A-large.jpg", "Evan A. Schmutz"));
            photos.Add(new Photo("Schweitzer-Gregory-A-large.jpg", "Gregory A. Schwitzer"));
            photos.Add(new Photo("Sitati-Joseph-W-large.jpg", "Joseph W. Sitati"));
            photos.Add(new Photo("Snow-Steven-E-large.jpg", "Steven E. Snow"));
            photos.Add(new Photo("Stanfill-Vern-P-large.jpg", "Vern P. Stanfill"));
            photos.Add(new Photo("Taylor-Brian-K-large.jpg", "Brian K. Taylor"));
            photos.Add(new Photo("Teh-Michael-John-U-large.jpg", "Michael John U. Teh"));
            photos.Add(new Photo("Teixeira-Jose-A-large.jpg", "José A. Teixeira"));
            photos.Add(new Photo("Valenzuela-Arnulfo-large.jpg", "Arnulfo Valenzuela"));
            photos.Add(new Photo("Vinson-Terence-M-large.jpg", "Terence M. Vinson"));
            photos.Add(new Photo("Wakolo-Taniela-B-large.jpg", "Taniela B. Wakolo"));
            photos.Add(new Photo("Whiting-Scott-D-large.jpg", "Scott D. Whiting"));
            photos.Add(new Photo("Wilson-Larry-Y-large.jpg", "Larry Y. Wilson"));
            photos.Add(new Photo("Wong-Chi-Hong-Sam-large.jpg", "Chi Hong (Sam) Wong"));
            photos.Add(new Photo("Yamashita-Kazuhiko-large.jpg", "Kazuhiko Yamashita"));
            photos.Add(new Photo("Zeballos-Jorge-F-large.jpg", "Jorge F. Zeballos"));
            photos.Add(new Photo("Zivic-Claudio-D-large.jpg", "Claudio D. Zivic"));
            photos.Add(new Photo("Zwick-W-Craig-large.jpg", "W. Craig Zwick"));
            photos.Add(new Photo("Ashton_Brian_Kent-Portrait-276x337.jpg", "Brian K. Ashton"));
            photos.Add(new Photo("bonnie-h-cordon-large-278x346-2017.jpg", "Bonnie H. Cordon"));
            photos.Add(new Photo("bonnie-oscarson-md-1190455.jpg", "Bonnie L. Oscarson"));
            photos.Add(new Photo("carol-mcconkie-1190461.jpg", "Carol F. McConkie"));
            photos.Add(new Photo("cristina-b-franco-large-278x346-2017.jpg", "Cristina B. Franco"));
            photos.Add(new Photo("devin-g-durrant-large.jpg", "Devin G. Durrant"));
            photos.Add(new Photo("douglas_d_holmes_large.jpg", "Douglas D. Holmes"));
            photos.Add(new Photo("jean-b-bingham-large-278x346-04-2017.jpg", "Jean B. Bingham"));
            photos.Add(new Photo("joy-d-jones-large-278x346-2017.jpg", "Joy D. Jones"));
            photos.Add(new Photo("M_Joseph_Brough_large.jpg", "M. Joseph Brough"));
            photos.Add(new Photo("neill-marriott-md-1190464.jpg", "Neill F. Marriott"));
            photos.Add(new Photo("reyna-i-aburto-large-278x346-2017.jpg", "Reyna I. Aburto"));
            photos.Add(new Photo("sharon-eubank-large-278x346-2017.jpg", "Sharon Eubank"));
            photos.Add(new Photo("stephen_w_owen_large.jpg", "Stephen W. Owen"));
            photos.Add(new Photo("tad-r-callister-large.jpg", "Tad R. Callister"));

            #endregion

            StorageFolder storageFolder = await KnownFolders.GetFolderForUserAsync(null, KnownFolderId.PicturesLibrary);

            foreach (var photo in photos)
            {
                StorageFile file = await storageFolder.GetFileAsync(photo.PhotoName);
                var personResult = await faceHelper.AddPerson("prophets", photo.PersonName);
                await faceHelper.AddImageToPerson("prophets", personResult.PersonId, file);
            }

            await faceHelper.TrainGroup("prophets");
        }

        private void FacesCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FacesCombo.Items == null || FacesCombo.Items.Count == 0) return;

            var people = FacesCombo.SelectedValue as List<Identification>;

            var faces = people.Select(p => p.Face).ToList();

            UpdateFaces(faces);
            Results.Text = JsonConvert.SerializeObject(faces, Formatting.Indented);
        }

        private async void UpdateFaces(List<Face> selectedFaces)
        {
            var imageSource = await _faceHelper.MarkFaces(_image, selectedFaces.ToArray());

            if (imageSource != null)
            {
                ImageDisplay.Source = imageSource;
            }
        }
    }
}
