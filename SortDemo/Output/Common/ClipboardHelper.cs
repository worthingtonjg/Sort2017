using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Common
{
    public class ClipboardHelper
    {
        public async Task<StorageFile> ImageFromClipboard()
        {
            StorageFile result = null;
            
            var dataPackageView = Clipboard.GetContent();
            
            if (dataPackageView.Contains(StandardDataFormats.Bitmap))
            {
                IRandomAccessStreamReference imageReceived = null;
           
                imageReceived = await dataPackageView.GetBitmapAsync();

                if (imageReceived == null)
                {
                    throw new Exception("No image in clipboard");
                }

                using (var imageStream = await imageReceived.OpenReadAsync())
                {
                    StorageFolder storageFolder = await KnownFolders.GetFolderForUserAsync(null, KnownFolderId.PicturesLibrary);

                    // Make the current copy
                    StorageFile storageFile1 = await storageFolder.CreateFileAsync("_clipboard.png", CreationCollisionOption.ReplaceExisting);
                    imageStream.Seek(0);
                    using (var stream = await storageFile1.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await imageStream.AsStreamForRead().CopyToAsync(stream.AsStreamForWrite());
                    }

                    // Make a historical copy
                    StorageFile storageFile2 = await storageFolder.CreateFileAsync("image.png", CreationCollisionOption.GenerateUniqueName);
                    imageStream.Seek(0);
                    using (var stream = await storageFile2.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await imageStream.AsStreamForRead().CopyToAsync(stream.AsStreamForWrite());
                    }

                    // Show the image
                    result = storageFile1;
                }
            }
            else
            {
                throw new Exception("No image in clipboard");
            }

            return result;
        }

        public async Task<BitmapImage> ImageSourceFromStorageFile(StorageFile image)
        {
            var imageSource = new BitmapImage();

            using (var stream = await image.OpenReadAsync())
            {
                
                imageSource.SetSource(stream);
            }

            return imageSource;
        }

        public void ImageToClipboard(StorageFile image)
        {
            var dataPackage = new DataPackage();

            dataPackage.RequestedOperation = DataPackageOperation.Copy;

            var stream = RandomAccessStreamReference.CreateFromFile(image);
            dataPackage.SetBitmap(stream);

            Clipboard.SetContent(dataPackage);
        }

    }
}

