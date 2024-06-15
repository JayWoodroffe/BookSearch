using BookSearch_UWP_fr.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.ViewManagement;

namespace BookSearch_UWP_fr
{
    public sealed partial class BookDetailsPage : Page
    {
        private BookDetailsViewModel _viewModel;

        //constructor initialises components and sets the data context 
        public BookDetailsPage()
        {
            InitializeComponent();
            this.DataContext = new BookDetailsViewModel();
            EnterFullScreenMode();
        }

        public BookDetailsViewModel BookDetailsViewModel
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

        //returns the user to the SearchPage (the frame loaded before this)
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        //when this page is loaded, it requires a BookModel paramater that will be passed to the ViewModel 
        //this will be the BookModel of the book that is clicked on in SearchPage
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is BookModel book)
            {
                if (DataContext is BookDetailsViewModel viewModel)
                {
                    viewModel.Book = book;
                }
            }
        }

        //When the cover image displayed in the page is clicked, this method will be called to enlarge it
        private void OnCoverImageTapped(object sender, TappedRoutedEventArgs e)
        {
            if (DataContext is BookDetailsViewModel viewModel)
            {
                var book = viewModel.Book;
                if (book != null)
                {
                    // Modify the size of the original image
                    ImageColumn.Width = new GridLength(550);
                    CoverImage.Width = 550;
                    CoverImage.Height = 700;
                }
            }
        }

        //when the author's name ic clicked, it will navigate to the AuthorPage and give it the authorKey as the parameter 
        private void OnAuthorNameTapped(object sender, TappedRoutedEventArgs e)
        {
            //get the author key from the Tag property of the tapped text block
            string authorKey = (sender as TextBlock)?.Tag?.ToString();

            if (!string.IsNullOrEmpty(authorKey))
            {
                // Navigate to the AuthorPage
                Frame.Navigate(typeof(AuthorPage), authorKey);
            }
        }



    }


}
