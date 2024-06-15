using BookSearch_UWP_fr.Models;
using BookSearch_UWP_fr.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BookSearch_UWP_fr
{
    public sealed partial class AuthorPage : Page
    {
        //constructor initialises components and sets data context
        public AuthorPage()
        {
            this.InitializeComponent();
            this.DataContext = new AuthorPageViewModel();
            EnterFullScreenMode();
        }

        public AuthorPageViewModel AuthorPageViewModel
        {
            get => default;
            set
            {
            }
        }

        //sets the page to full screen
        private void EnterFullScreenMode()
        {
            ApplicationView view = ApplicationView.GetForCurrentView();
            view.TryEnterFullScreenMode();
        }
        //triggered when the Visit Website Hyperlink is clicked in order to open the URL
        private async void OnHyperlinkClicked(object sender, RoutedEventArgs e)
        {
            var viewModel = (AuthorPageViewModel)DataContext;

            if (viewModel != null)
            {
                bool success = await viewModel.OpenAuthorUrlAsync(); //calls the method in the ViewModel

                if (!success)
                {
                    Debug.WriteLine("Failed to open author URL");
                    // Handle failure to open URL
                }
            }
            else
            {
                Debug.WriteLine("DataContext is not an instance of AuthorPageViewModel");
            }
        }

        //returns the user to the BookDetails page
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack(); 
        }

        /*when the page is opened, the authorkey is retrieved from the paramater and sent to the ViewModel 
         * in order to initialise the rest of the author's data*/
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Check if navigation parameter is an author key
            if (e.Parameter is string authorKey)
            {
                // Set AuthorKey property of AuthorPageViewModel
                if (DataContext is AuthorPageViewModel viewModel)
                {
                    viewModel.AuthorKey = authorKey;
                }
            }
        }
    }
}
