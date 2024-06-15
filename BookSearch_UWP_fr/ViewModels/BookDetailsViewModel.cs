using BookSearch_UWP_fr.Models;
using BookSearch_UWP_fr.Services;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BookSearch_UWP_fr.ViewModels
{
    public class BookDetailsViewModel : INotifyPropertyChanged
    {
        private BookModel _book;
        private OpenLibraryService _openLibraryService;

        //constructor creates an instance of the data acces service class
        public BookDetailsViewModel()
        {
            _openLibraryService = new OpenLibraryService();
        }

        //Book holds the object information we will display in this page
        public BookModel Book
        {
            get => _book;
            set
            {
                if (_book != value) //check if the value is different
                {
                    _book = value; //assign value to private _book
                    
                    OnPropertyChanged(nameof(Book)); //notify UI that Book property has changed
                    _ = LoadBookAsync(); // Start loading book asynchronously
                }
            }
        }

        //This method is required to get the description for the book
        //all the other data has been passed to this method with the Book BookModel
        //the description is queried using another API query with its Key
        public async Task LoadBookAsync()
        {
            Book.Description = await _openLibraryService.GetDescription(Book.Key);//get the string description
            OnPropertyChanged(nameof(Book)); // Notify UI that Book property has changed (description has been updated)
        }


        public event PropertyChangedEventHandler PropertyChanged;

        //property change notification
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
