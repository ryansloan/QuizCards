using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace QuizCards
{
    public class Card : INotifyPropertyChanged
    {
        private String _sideALabel;
        private String _sideBLabel;
        public String sideALabel
        {
            get
            {
                return this._sideALabel;
            }
            set
            {
                if (value == this._sideALabel)
                {
                    return;
                }
                else
                {
                    this._sideALabel = value;
                    OnPropertyChanged("sideALabel");
                }
            }
        }
        public String sideBLabel
        {
            get
            {
                return this._sideBLabel;
            }
            set
            {
                if (value==this._sideBLabel)
                {
                    return;
                }
                else
                {
                    this._sideBLabel = value;
                    OnPropertyChanged("sideBLabel");
                }
            }
        }
        private BitmapImage _sideAImage;
        public BitmapImage sideAImage
        {
            get
            {
                return this._sideAImage;
            }
        }
        private BitmapImage _sideBImage;
        public BitmapImage sideBImage
        {
            get
            {
                return this._sideBImage;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public Card()
        {
            this.sideALabel = null;
            this.sideBLabel = null;
            this._sideAImage = null;
            this._sideBImage = null;
        }

        public Card(String sA, String sB)
        {
            this.sideALabel = sA;
            this.sideBLabel = sB;
            this._sideAImage = null;
            this._sideBImage = null;
        }
        public Card(String sA, String sB, BitmapImage i1, BitmapImage i2)
        {
            this.sideALabel = sA;
            this.sideBLabel = sB;
            this._sideAImage = i1;
            this._sideBImage = i2;
        }
        public Card(String sA, String sB, String i1, String i2)
        {
            this.sideALabel = sA;
            this.sideBLabel = sB;
            this._sideAImage = new BitmapImage();
            this._sideAImage.UriSource = new Uri(i1);
            this._sideAImage = new BitmapImage();
            this._sideAImage.UriSource = new Uri(i2);

        }
        public bool hasImages()
        {
            return ((this._sideAImage != null) || (this._sideBImage != null));
        }
        public bool hasSideAImage()
        {
            return (this._sideAImage != null);
        }
        public bool hasSideBImage()
        {
            return (this._sideBImage != null);
        }
        public void setSideAImage(String s)
        {
            this._sideAImage = new BitmapImage();
            this._sideAImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            this._sideAImage.UriSource = new Uri(s);
        }
        public void setSideAImage(BitmapImage bi)
        {
            this._sideAImage = bi;
        }
        public void setSideBImage(String s)
        {
            this._sideBImage = new BitmapImage();
            this._sideBImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            this._sideBImage.UriSource = new Uri(s);
        }
        public void setSideBImage(BitmapImage bi)
        {
            this._sideBImage = bi;
        }
        public string toString()
        {
            return this.sideALabel + " - " + this.sideBLabel;
        }
        public void emptyBitmap()
        {
            if (this._sideAImage != null)
            {
                this._sideAImage.UriSource = null;
                this._sideAImage = null;
            }
            if (this._sideBImage != null)
            {
                this._sideBImage.UriSource = null;
                this._sideBImage = null;
            }
        }
    }
}
