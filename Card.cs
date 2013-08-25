using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace QuizCards
{
    public class Card
    {
        public String sideALabel { get; set; }
        public String sideBLabel { get; set; }
        private BitmapImage image;

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
    }
}
