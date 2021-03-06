﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            CardsGridView.ItemTemplate = SideATemplate;
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
                if (p.ContainsKey("Card"))
                {
                    CardsGridView.SelectedItem = p["Card"] as Card;
                    CardsGridView.Loaded += scrollToSelected;
                }

            }
            else
            {
                currentDeck = new Deck();
                this.currentDeck.title = "My Deck Name";
                this.currentDeck.sideAName = "Side A";
                this.currentDeck.sideBName = "Side B";
                this.DataContext = this.currentDeck;
            }


        }
        private void scrollToSelected(object sender, RoutedEventArgs args)
        {
            CardsGridView.ScrollIntoView(CardsGridView.SelectedItem,ScrollIntoViewAlignment.Leading);
        }
        private void BackBtn_Tapped(object sender, RoutedEventArgs e)
        {
            //autosave deck
            if (this.currentDeck.filename == null)
            {
                this.currentDeck.filename = this.currentDeck.title + ".qcd";
            }
            SaveDeckFromFileName();
            //Viewing a deck->Quizzing->View blows up backstack, so use Navigate instead.
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
            if (CardsGridView.SelectedItem != null)
            {
                EditCardBtn.IsEnabled = true;
            }
            else
            {
                EditCardBtn.IsEnabled = false;
            }
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
                CardsGridView.ItemTemplate = SideATemplate;
                this.frontSide = "A";
            }
            else
            {
                CardsGridView.ItemTemplate = SideBTemplate;
                this.frontSide = "B";
            }
        }

        private void EditDeckBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Dictionary<String, Object> p = new Dictionary<String, Object>();
            p.Add("Deck", this.currentDeck);
            this.Frame.Navigate(typeof(DeckEditPage), p);
        }

        private void AddCardBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Dictionary<String, Object> p = new Dictionary<String, Object>();
            p.Add("Deck", this.currentDeck);
            this.Frame.Navigate(typeof(CardEditPage), p);
        }

        private void EditCardBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Dictionary<String, Object> p = new Dictionary<String, Object>();
            p.Add("Deck", this.currentDeck);
            p.Add("Card", CardsGridView.SelectedItem as Card);
            this.Frame.Navigate(typeof(CardEditPage), p);
        }

        private void SaveDeckBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PickSaveFile();
        }
        private async void SaveDeckFromFileName() {
            var folder = ApplicationData.Current.LocalFolder;
            StorageFile outfile = await folder.CreateFileAsync(this.currentDeck.filename);
            await SaveDeck(outfile);
            this.currentDeck.disposeOfBitmaps();
        }
        private async Task<bool> SaveDeck(StorageFile file)
        {
            if (file != null)
            {
                DeckPackageProcessor dpp = new DeckPackageProcessor();
                return await dpp.writePackageAsync(file, this.currentDeck);
            }
            else
            {
                return false;
            }
        }
        private async void PickSaveFile()
        {
            FileSavePicker picker = new FileSavePicker();
            picker.FileTypeChoices.Add("QuizCard Deck", new string[] { ".qcd" });
            StorageFile file = await picker.PickSaveFileAsync();
            SaveDeck(file);
        }

    }
}
