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
    public sealed partial class LandingPage : Page
    {
        public LandingPage()
        {
            this.InitializeComponent();
        }

        private async void OpenDeckFromFile()
        {

            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".zip");
            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                Dictionary<String, Object> p = new Dictionary<String, Object>();
                p.Add("DeckFile", file);
                this.Frame.Navigate(typeof(DeckSummaryPage), p);
            }

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void DeckLoadBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            OpenDeckFromFile();
        }

        private void CreateDeckBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DeckSummaryPage));
        }
    }
}
