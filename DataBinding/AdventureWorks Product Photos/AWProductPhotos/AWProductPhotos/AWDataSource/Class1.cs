using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Reflection;

namespace AWDataSource
{
    public class ProductPhoto
    {
        public ProductPhoto(int id, byte[] thumbNailPhoto, byte[] largePhoto, DateTime modifiedDate)
        {
            this.id = id;
            this.thumbNailPhoto = ByteArrayToImageSource(thumbNailPhoto);
            this.largePhoto = ByteArrayToImageSource(largePhoto);
            this.modifiedDate = modifiedDate;
        }
        public int ID
        {
            get { return id; }
        } int id;
        public ImageSource ThumbNailPhoto
        {
            get { return thumbNailPhoto; }
        } ImageSource thumbNailPhoto;
        public ImageSource LargePhoto
        {
            get { return largePhoto; }
        } ImageSource largePhoto;
        public DateTime ModifiedDate
        {
            get { return modifiedDate; }
        } DateTime modifiedDate;
        private ImageSource ByteArrayToImageSource(byte[] data)
        {
            BitmapImage image = null;
            if (data != null)
            {
                image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = new System.IO.MemoryStream(data);
                image.EndInit();
            }
            return image;
        }
    }

    public class ProductPhotosCollection
    {
        private ObservableCollection<ProductPhoto> productPhotos = new ObservableCollection<ProductPhoto>();
        private DelegateCommand getDataCommand;
        public ProductPhotosCollection()
        {
            getDataCommand = new DelegateCommand(delegate() { GetData(); });
        }
        public DelegateCommand GetDataCommand { get { return getDataCommand; } }
        private void GetData()
        {
            // uncomment the following two lines to load data from the Adventureworks database.
            //ProductPhotosTableAdapters.ProductPhotoTableAdapter da = new ProductPhotosTableAdapters.ProductPhotoTableAdapter();
            //ProductPhotos.ProductPhotoDataTable dt = da.GetData();

            // If you uncomment the above two lines then comment the next two out.
            ProductPhotos.ProductPhotoDataTable dt = new ProductPhotos.ProductPhotoDataTable();
            dt.ReadXml(Assembly.GetExecutingAssembly().GetManifestResourceStream("AWDataSource.productphotos.xml"));

            productPhotos.Clear();
            foreach (ProductPhotos.ProductPhotoRow row in dt)
            {
                productPhotos.Add(new ProductPhoto(row.ProductPhotoID, row.ThumbNailPhoto, row.LargePhoto, row.ModifiedDate));
            }
        }
        public ObservableCollection<ProductPhoto> ProductPhotos
        {
            get { return this.productPhotos; }
        }
    }
}
