using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class DeckSummaryPage : Page
    {

        private Deck currentDeck;
        private String frontSide = "A";
        public DeckSummaryPage()
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
                if (p.ContainsKey("DeckFile"))
                {
                    loadDeckFromFile(p["DeckFile"] as StorageFile);
                }
                else if (p.ContainsKey("Deck"))
                {
                    this.currentDeck = p["Deck"] as Deck;
                    this.DataContext = this.currentDeck;
                }

            }
            else
            {
                currentDeck = new Deck();
                this.currentDeck.sideAName = "SideA";
                this.currentDeck.sideBName = "SideB";
                this.DataContext = this.currentDeck;
            }

        }

        private void BackBtn_Tapped(object sender, RoutedEventArgs e)
        {
            //Viewing a deck->Quizzing->View blows up backstack, so use Navigate instead.
            this.currentDeck.disposeOfBitmaps();
            this.Frame.Navigate(typeof(LandingPage));
        }

        private async void loadDeckFromFile(StorageFile file)
        {
            //File Picker, feed to processor, wait and show progress
            DeckPackageProcessor dpp = new DeckPackageProcessor();
            progressRing.IsEnabled = true;
            progressRing.IsActive = true;
            if (await dpp.readPackageAsync(file))
            {
                //Once we're done, kill progress, set up controls, and update the display
                progressRing.IsEnabled = false;
                progressRing.IsActive = false;
                this.currentDeck = dpp.deck;
                this.DataContext = this.currentDeck;
                CardsGridView.ItemTemplate = ShowSideATemplate;
            }
            else
            {
                Windows.UI.Popups.MessageDialog errorDialog = new Windows.UI.Popups.MessageDialog("There was a problem loading the file you selected. Make sure you've selected a valid QuizCards deck.", "Uh oh! I couldn't load that file.");
                await errorDialog.ShowAsync();
                Frame.Navigate(typeof(LandingPage));
            }
            //CardsGridView.DataContext = this.currentDeck;       
        }

        private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SummaryAppBar.IsOpen = true;
        }

        private void StartQuizBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Dictionary<String, Object> p = new Dictionary<String, Object>();
            p.Add("Deck", this.currentDeck);
            this.Frame.Navigate(typeof(MainPage), p);
        }

        private void FlipCardbtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (this.frontSide == "B")
            {
                CardsGridView.ItemTemplate = ShowSideATemplate;
                this.frontSide = "A";
            }
            else
            {
                CardsGridView.ItemTemplate = ShowSideBTemplate;
                this.frontSide = "B";
            }
        }
    }
}
