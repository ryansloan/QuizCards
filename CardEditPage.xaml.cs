using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace QuizCards
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CardEditPage : Page
    {
        private Deck currentDeck;
        private Card currentCard;
        private Boolean isNewCard = false;
        public CardEditPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                Dictionary<String, Object> p = e.Parameter as Dictionary<String, Object>;
                if (p.ContainsKey("Deck"))
                {
                    this.currentDeck = p["Deck"] as Deck;
                    this.DataContext = this.currentDeck;
                    if (p.ContainsKey("Card"))
                    {
                        this.currentCard = p["Card"] as Card;
                    }
                    else
                    {
                        this.currentCard = new Card();
                        this.isNewCard = true;
                    }
                    SideAStackPanel.DataContext = this.currentCard;
                    SideBStackPanel.DataContext = this.currentCard;
                    if (this.currentCard.hasSideAImage()) {
                        ClearSideAImgBtn.IsEnabled = true;
                    }
                    if (this.currentCard.hasSideBImage())
                    {
                        ClearSideBImgBtn.IsEnabled = true;
                    }
                }
                else
                {
                    ShowErrorAndGoBack("The current deck is invalid");
                }

            }
            else
            {
                ShowErrorAndGoBack("The current deck is invalid");
            }

        }
        public async void ShowErrorAndGoBack(string err)
        {
            Windows.UI.Popups.MessageDialog errorDialog = new Windows.UI.Popups.MessageDialog(err,"Whoops, there's been a problem");
            await errorDialog.ShowAsync();
            Frame.Navigate(typeof(LandingPage));
        }
        private void BackBtn_Tapped(object sender, RoutedEventArgs e)
        {
            //Viewing a deck->Quizzing->View blows up backstack, so use Navigate instead.
            Dictionary<String, Object> p = new Dictionary<String, Object>();

            if (this.isNewCard)
            {
                if ((this.currentCard.sideALabel != null) || (this.currentCard.sideBLabel != null))
                {
                    this.currentDeck.addCard(this.currentCard);
                    p.Add("Card", this.currentCard);
                }
            }
            else
            {
                p.Add("Card",this.currentCard);
            }
            p.Add("Deck", this.currentDeck);
            this.Frame.Navigate(typeof(DeckSummaryPage),p);
        }

        private void InsertImageFromFileSideA_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PickAndCopyImage("A");
        }
        private async void PickAndCopyImage(string side)
        {
            var tmpFolder = ApplicationData.Current.TemporaryFolder;
            string outFileName = Guid.NewGuid().ToString();
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            StorageFile inFile = await picker.PickSingleFileAsync();
            //copy to local tmp directory
            StorageFile outfile = null;

            using (Stream s = await inFile.OpenStreamForReadAsync())
            {
                outFileName += inFile.FileType;
                outfile = await tmpFolder.CreateFileAsync(outFileName, CreationCollisionOption.ReplaceExisting);
                using (Stream sout = await outfile.OpenStreamForWriteAsync())
                {
                    await s.CopyToAsync(sout);
                    await sout.FlushAsync();
                }
            }
            if (side == "A")
            {
                this.currentCard.setSideAImage("ms-appdata:///temp/" + outFileName);
                SideAImage.Source = this.currentCard.sideAImage;
                ClearSideAImgBtn.IsEnabled = true;
            }
            else if (side == "B")
            {
                this.currentCard.setSideBImage("ms-appdata:///temp/" + outFileName);
                SideBImage.Source = this.currentCard.sideBImage;
                ClearSideBImgBtn.IsEnabled = true;
            }
        }

        private void InsertImageFromFileSideB_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PickAndCopyImage("B");
        }

        private void ClearSideAImgBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.currentCard.clearBitmap("A");
            ClearSideAImgBtn.IsEnabled= false;
        }

        private void ClearSideBImgBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.currentCard.clearBitmap("B");
            ClearSideBImgBtn.IsEnabled = false;
        }
    }
}
