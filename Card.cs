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
                if (value == this._sideBLabel)
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
        private BitmapImage image;


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public Card()
        {
            this.sideALabel = null;
            this.sideBLabel = null;
            this.image = null;
        }

        public Card(String sA, String sB)
        {
            this.sideALabel = sA;
            this.sideBLabel = sB;
            this.image = null;
        }
        public Card(String sA, String sB, BitmapImage i)
        {
            this.sideALabel = sA;
            this.sideBLabel = sB;
            this.image = i;
        }
        public Card(String sA, String sB, String i)
        {
            this.sideALabel = sA;
            this.sideBLabel = sB;
            this.image = new BitmapImage();
            this.image.UriSource = new Uri(i);

        }
        public bool hasImage()
        {
            return this.image != null;
        }
        public BitmapImage getImage()
        {
            return this.image;
        }
        public void setImage(String s)
        {
            this.image = new BitmapImage();
            this.image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            this.image.UriSource = new Uri(s);
        }
        public void setImage(BitmapImage bi)
        {
            this.image = bi;
        }
        public string toString()
        {
            return this.sideALabel + " - " + this.sideBLabel;
        }
        public void emptyBitmap()
        {
            if (this.image != null)
            {
                this.image.UriSource = null;
                this.image = null;
            }
        }
    }
}
