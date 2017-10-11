using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Common
{
    public class UIHelper
    {
        private DispatcherTimer _timer = new DispatcherTimer();
        private ScrollViewer _scrollViewer;
        private Image _image;
        
        public void SetZoom(ScrollViewer scrollViewer, Image image)
        {
            _scrollViewer = scrollViewer;
            _image = image;

            _timer.Interval = new System.TimeSpan(0, 0, 1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            if (_image.ActualWidth == 0 || _image.ActualHeight == 0) return;

            _timer.Stop();

            var scrollViewerWidth = _scrollViewer.ActualWidth;
            var scrollViewerHeight = _scrollViewer.ActualHeight;
            var imageWidth = _image.ActualWidth;
            var imageHeight = _image.ActualHeight;
            double scaleFactor = 1;

            double wideScaleFactor = scrollViewerWidth / imageWidth;
            double heightScaleFactor = scaleFactor = scrollViewerHeight / imageHeight;

            if (wideScaleFactor < heightScaleFactor)
                scaleFactor = wideScaleFactor;
            else
                scaleFactor = heightScaleFactor;

            _scrollViewer.ChangeView(0, 0, (float)scaleFactor);
        }
    }
}
