using BookSearch_UWP_fr.Models;
using BookSearch_UWP_fr.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BookSearch_UWP_fr
{ 
    public sealed partial class MainPage : Page
    {
        //to check if this is the page's first load 
        private static bool isFirstLoad = true;
        
        //constructor initialises components and sets Data context
        public MainPage()
        {
            
            this.InitializeComponent();
            this.DataContext = new SearchPageViewModel();
            EnterFullScreenMode();
        }

        public SearchPageViewModel SearchPageViewModel
        {
            get => default;
            set
            {
            }
        }

        //sets page to full screen
        private void EnterFullScreenMode()
        {
            ApplicationView view = ApplicationView.GetForCurrentView();
            view.TryEnterFullScreenMode();
        }

        //when the page is opened
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (DataContext is SearchPageViewModel searchPageViewModel)
            {
                //if this page is loaded to for the first time (ie application just opened), nothing happens
                //if the search page is being returned to from another page, loads most recent search from the history
                if (isFirstLoad)
                {
                    isFirstLoad = false;
                    return;
                }
                _ = searchPageViewModel.ExecuteMostRecentSearchAsync();

            }
        }

        //if an item in the list is clicked, sends that book's BookModel to the BookDetails page
        private void OnBookSelected(object sender, ItemClickEventArgs e)
        {
            // clicked item as a BookModel
            var selectedBook = e.ClickedItem as BookModel;
            if (selectedBook != null)
            {
                // Navigate to the BookDetailsPage, passing the selected book's information
                Frame.Navigate(typeof(BookDetailsPage), selectedBook);
            }
        }

        
    }
}
