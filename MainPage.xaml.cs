using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using System.Diagnostics;
using Windows.Storage;
using Windows.Storage.Pickers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace QuizCards
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public Deck currentDeck = null;
        public Card currentCard = null;
        private Point initialPt;
        private bool enableManipulation=false;
        public MainPage()
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
            //For now just kick off the load operations.
            loadData();
        }
        private async void loadData() {
            //File Picker, feed to processor, wait and show progress
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".zip");
            StorageFile file = await picker.PickSingleFileAsync();
            DeckPackageProcessor dpp = new DeckPackageProcessor();
            progring.IsEnabled = true;
            progring.IsActive = true;
            await dpp.readPackageAsync(file);
            //Once we're done, kill progress, set up controls, and update the display
            progring.IsEnabled = false;
            progring.IsActive = false;
            NextCardBtn.Visibility = Visibility.Visible;
            PrevCardBtn.Visibility = Visibility.Visible;
            FlipCardBtn.Visibility = Visibility.Visible;
            this.currentDeck = dpp.deck;
            this.currentDeck.shuffle();
            DeckTitleBlock.Text = this.currentDeck.getTitle();
            this.currentCard=this.currentDeck.getNextCard();
            FlipCardBtn.Content = "Show " + this.currentDeck.getSideBName();
            this.updateVisibleCard();
        }

        private void NextCardBtn_Click(object sender, RoutedEventArgs e)
        {
                this.currentCard = this.currentDeck.getNextCard();
                this.updateVisibleCard();
        }
        private void updateVisibleCard()
        {
            SideBLabel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            SideALabel.Text = this.currentCard.sideALabel;
            SideBLabel.Text = this.currentCard.sideBLabel;
            if (this.currentCard.hasImage())
            {
                SideAImage.Source = this.currentCard.getImage();
            }
            else
            {
                SideAImage.Source = null;
            }
            if (this.currentDeck.isNextCard()) { this.NextCardBtn.IsEnabled=true; }
            else { this.NextCardBtn.IsEnabled=false; }
            if (this.currentDeck.isPreviousCard()) { this.PrevCardBtn.IsEnabled = true; }
            else { this.PrevCardBtn.IsEnabled = false; }
        }

        private void PrevCardBtn_Click(object sender, RoutedEventArgs e)
        {

                this.currentCard = this.currentDeck.getPreviousCard();
                this.updateVisibleCard();

        }

        private void FlipCardBtn_Click(object sender, RoutedEventArgs e)
        {
            SideBLabel.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void Card_ManipulationDelta (object sender, ManipulationDeltaRoutedEventArgs e)
        {
            int swipeThreshold = 250;
            if ((e.IsInertial) && (this.enableManipulation))
            {
                Point curr = e.Position;
                if ((curr.X-this.initialPt.X > swipeThreshold) && (this.currentDeck.isPreviousCard()))
                {
                this.enableManipulation = false;
                this.currentCard = this.currentDeck.getPreviousCard();
                this.updateVisibleCard();
                }
                if ((this.initialPt.X - curr.X > swipeThreshold)&&(this.currentDeck.isNextCard()))
                {
                this.enableManipulation = false;
                this.currentCard = this.currentDeck.getNextCard();
                this.updateVisibleCard();
                }
            }
        }

        private void Card_ManipulationStarted (object sender, ManipulationStartedRoutedEventArgs e)
        {
            if (e.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
            {
                this.initialPt = e.Position;
                this.enableManipulation = true;
            }
            else
            {
                this.enableManipulation = false;
            }
        }
        private void Card_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            this.enableManipulation = false;
        }

        private void Card_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (SideBLabel.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                SideBLabel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                SideBLabel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }
    }
}
