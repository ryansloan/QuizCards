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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace QuizCards
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DeckEditPage : Page
    {
        private Deck currentDeck;


        public DeckEditPage()
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
                }

            }
            else
            {
                currentDeck = new Deck();
                this.currentDeck.title = "My new deck";
                this.currentDeck.sideAName = "Side A";
                this.currentDeck.sideBName = "Side B";
                this.DataContext = this.currentDeck;
            }

        }

        private void BackBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Using Navigate rather than GoBack so we can retain the current deck in memory
            Dictionary<String, Object> p = new Dictionary<string, Object>();
            p.Add("Deck",this.currentDeck);
            this.Frame.Navigate(typeof(DeckSummaryPage), p); 
        }
    }
}
