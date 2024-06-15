using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookSearch_UWP_fr.ViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Diagnostics;
    using BookSearch_UWP_fr.Services;
    using Windows.Storage;
    using BookSearch_UWP_fr.Models;
    using System.IO;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// ViewModel class responsible for managing the book search page functionality and data.
    /// Implements the INotifyPropertyChanged interface to support data binding.
    /// Uses RelayCommand class to execute a Method when the view requestr
    /// </summary>
    public class SearchPageViewModel : INotifyPropertyChanged
    {
        private string _searchText;
        private string _selectedSearchCriteria;
        private ObservableCollection<BookModel> _searchResults;
        private ICommand _searchCommand;
        private readonly OpenLibraryService _openLibraryService;

        public event PropertyChangedEventHandler PropertyChanged;

        /*Constructor sets up global variables that are needed for this page*/
        public SearchPageViewModel()
        {
            SearchResults = new ObservableCollection<BookModel>(); //will store the BookModels that result from the search
            SearchCriteriaOptions = new List<string> {"No search criteria", "Title", "Author", "Subject" }; //sets the combobox options
            SelectedSearchCriteria = "No Search Criteria";//sets the currently selected search criteria

            //search command is triggered by the search button and executes SearchBooksAsync
            SearchCommand = new RelayCommand(async () => await SearchBooksAsync());

            _openLibraryService = new OpenLibraryService(); //instance of data access class 
        }

        //property for storing the searchText provided by the user
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }

        //property for storing the selected search option from the combobox
        public string SelectedSearchCriteria
        {
            get => _selectedSearchCriteria;
            set
            {
                _selectedSearchCriteria = value;
                OnPropertyChanged();
            }
        }

        //property to store the search results as book models
        public ObservableCollection<BookModel> SearchResults
        {
            get => _searchResults;
            set
            {
                _searchResults = value;
                OnPropertyChanged();
            }
        }

        //returning the SearchCriteriaOptions that were set up in the constructor
        public List<string> SearchCriteriaOptions { get; }

        //Command that will trigger the book search
        public ICommand SearchCommand
        {
            get => _searchCommand;
            set
            {
                _searchCommand = value;
                OnPropertyChanged();
            }
        }

        //property to store if the application is currently loading data/ waiting for data from an await statement
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        //property change notification
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //book search method checks if the search term is not null and then calls the OpenLibraryService method to retrieve the data
        private async Task SearchBooksAsync()
        {

            IsLoading = true;

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                IsLoading = false;
                return;
            }

            try
            {
                await SaveSearchHistory(); //saves this search using logical storage
                SearchResults.Clear();
                var results = await _openLibraryService.SearchBooks(SearchText, SelectedSearchCriteria); //gets the results from the search

                
                foreach (var result in results)
                {
                    SearchResults.Add(result);//adds the results to the observable collection
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /*This method uses local storage to save the search history of the user
         Creates a SearchHistoryItemModel from the information, serialises it and adds it to the folder*/
        public async Task SaveSearchHistory()
        {
            var searchHistoryItem = new SearchHistoryItemModel
            {
                SearchTerm = SearchText,
                SearchCriteria = SelectedSearchCriteria,
                Timestamp = DateTime.Now
            };

            //serialise search history item to json
            string searchHistoryJson = JsonConvert.SerializeObject(searchHistoryItem);

            //get local folder for app data
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            //create or open the search history file
            StorageFile file = await localFolder.CreateFileAsync("searchHistory.json", CreationCollisionOption.OpenIfExists);

            //read existing JSON content from the file
            string existingJson = await FileIO.ReadTextAsync(file);

            //remove the trailing ']' to append new data
            string trimmedJson = existingJson.TrimEnd(']').Trim();

            //Add comma
            if (!string.IsNullOrEmpty(trimmedJson))
            {
                trimmedJson += ",";
            }
            else
            {
                trimmedJson = "[" + trimmedJson ;
            }
            //append the new data
            string updatedJson = $"{trimmedJson}{searchHistoryJson}]";

            //overwrite the file with the new data
            await FileIO.WriteTextAsync(file, updatedJson);
        }


        //Retrieves the last search from the searchHistory.json file and returns it as a SearchHistoryItem
        public async Task<SearchHistoryItemModel> GetMostRecentSearchQuery()
        {
            SearchHistoryItemModel mostRecent = null;

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            try
            {
                StorageFile file = await localFolder.GetFileAsync("searchHistory.json");

                //read the file
                string searchHistoryJson = await FileIO.ReadTextAsync(file);

                //deserialize the JSON string to get an array of search history items
                SearchHistoryItemModel[] searchHistory = JsonConvert.DeserializeObject<SearchHistoryItemModel[]>(searchHistoryJson);

                //select the most recent search from the array
                if (searchHistory != null && searchHistory.Length > 0)
                {
                    mostRecent = searchHistory[searchHistory.Length - 1];
                }
            }
            catch (FileNotFoundException)
            {
                return null;
            }

            return mostRecent;
        }

        //reruns the query of the most recent search
        //for when the search page is navigated back to from book details page
        public async Task ExecuteMostRecentSearchAsync()
        {
            try
            {
                var recent = await GetMostRecentSearchQuery(); //retrieves the most recent query
                if (recent != null) //if there is one
                {
                    SearchText = recent.SearchTerm; //sets SearchText to the text in the recent query
                    SelectedSearchCriteria = recent.SearchCriteria;
                    //Debug.WriteLine($"search history item {SearchText}  {SelectedSearchCriteria} ");
                    await SearchBooksAsync(); // executes the method to retrieve the books based on query information
                }
                else
                {
                    Debug.WriteLine("No recent search query found.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred while executing most recent search: {ex.Message}");
            }
        }

        public RelayCommand RelayCommand
        {
            get => default;
            set
            {
            }
        }

        public SearchHistoryItemModel SearchHistoryItemModel
        {
            get => default;
            set
            {
            }
        }
    }
}