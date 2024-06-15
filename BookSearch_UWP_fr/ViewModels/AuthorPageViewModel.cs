using BookSearch_UWP_fr.Models;
using BookSearch_UWP_fr.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace BookSearch_UWP_fr.ViewModels
{
    using System.Diagnostics;
    using Windows.System;

    /// <summary>
    /// ViewModel class responsible for managing the Author Details Page and its functionality.
    /// Implements the INotifyPropertyChanged interface to support data binding.
    /// </summary>
    public class AuthorPageViewModel : INotifyPropertyChanged
    {
        private string _authorKey;
        private OpenLibraryService _openLibraryService;

        //constructor initialises data access object
        public AuthorPageViewModel()
        {
            _openLibraryService = new OpenLibraryService();
        }

        //variable and property to store the authors name
        private string _authorName;
        public string AuthorName
        {
            get => _authorName; 
            set {
                _authorName = value;
                OnPropertyChanged();

            }
        }

        //variable and property for authorUrl 
        private string _authorUrl;
        public string AuthorUrl
        {
            get => _authorUrl;
            set
            {
                _authorUrl = value;
                OnPropertyChanged(nameof(_authorUrl));
                OnPropertyChanged(nameof(UrlVisibility)); //updates the UrlVisibility
            }
        }
        //Url visibility checks if there is a AuthorUrl, if so, displays it, if not, collapses the HyperLink
        public Visibility UrlVisibility => string.IsNullOrEmpty(AuthorUrl) ? Visibility.Collapsed : Visibility.Visible;

        //variable and property for storing the author's date of birth
        private String _birthDate;
        public String BirthDate
        {
            get => _birthDate; 
            set {
                _birthDate = value;
                OnPropertyChanged();
            }
        }

        //variable and property for storing the authors biography
        private String _bio;
        public String Bio
        {
            get => _bio;
            set
            {
                _bio = value;
                OnPropertyChanged();
            }
        }

        //variable and property for storing the key that is given to this ViewModel by the View
        public string AuthorKey
        {
            get { return _authorKey; }
            set
            {
                if (_authorKey != value)
                {
                    _authorKey = value;
                    OnPropertyChanged();
                    //call method to fetch author data when author key changes
                    FetchAuthorDataAsync();
                }
            }
        }


        // Call the asynchronous method to fetch author data
        private async void FetchAuthorDataAsync()
        {
            if (!string.IsNullOrEmpty(AuthorKey))
            {
                
                await FetchAuthorData();
            }
        }
        // Method to fetch author details from the API
        private async Task FetchAuthorData()
        {
            //make API request to fetch author details using AuthorKey
            AuthorModel author= await _openLibraryService.GetAuthorDetailsAsync(_authorKey);
            
            // Update properties in this ViewModel with retrieved data
            if (author!= null)
            {
                AuthorName = author.Name;
                BirthDate = author.BirthDate;
                Bio = author.Bio;
                AuthorUrl = author.Url;
                Debug.Write("url in vm: " + AuthorUrl);
            }
            else
            {
                Bio = "Couldn't retrieve author's information.";
            }
        }

        //method is called when the author's url (HyperLink) is clicked to open their website
        public async Task<bool> OpenAuthorUrlAsync()
        {
            if (string.IsNullOrEmpty(AuthorUrl))
            {
                Debug.WriteLine("URL is null or empty");
                return false;
            }

            Uri uri = new Uri(AuthorUrl);

            bool success = await Launcher.LaunchUriAsync(uri); //launches the website

            if (!success)
            {
                Debug.WriteLine("Failed to launch URI");
            }

            return success;
        }

        //property change notification

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public AuthorModel AuthorModel
        {
            get => default;
            set
            {
            }
        }
    }
}
