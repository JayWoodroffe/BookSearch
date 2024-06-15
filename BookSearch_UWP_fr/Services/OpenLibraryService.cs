using BookSearch_UWP_fr.Models;
using BookSearch_UWP_fr.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BookSearch_UWP_fr.Services
{
    public class OpenLibraryService
    {

        private readonly HttpClient _httpClient;

        //constructor initialises HttpClient variable
        public OpenLibraryService()
        {
            _httpClient= new HttpClient();
        }

        //method searches for books based on search criteria 
        public async Task<List<BookModel>>SearchBooks(string searchText, string selectedCriteria )
        {
            string searchUrl;
            if (selectedCriteria.Equals("No Search Criteria"))//if no search criteria is specified
            {
                searchUrl = $"https://openlibrary.org/search.json?q={Uri.EscapeDataString(searchText)}";
            }
            else//query using search criteria
            {

                searchUrl = $"https://openlibrary.org/search.json?{selectedCriteria.ToLower()}={searchText}";
            }
            
            //gets the HttpResponse
            HttpResponseMessage response = await _httpClient.GetAsync(searchUrl);
            var searchResults = new List<BookModel>(); //creates a list of BookModels to store the results
            if (response.IsSuccessStatusCode)
            {
                string responseJson = await response.Content.ReadAsStringAsync(); //gets the response as a json string
                //deserialising the json
                dynamic searchData = JsonConvert.DeserializeObject<dynamic>(responseJson);

                var docs = searchData.docs;
                

                foreach( var doc in docs ) {
                    //retrieving the authorkey used to identify authors 
                    string authorKey = null;
                    JArray authorKeys = new JArray(); 

                    if (doc.ContainsKey("author_key") && doc["author_key"] is JArray)
                    {
                        authorKeys = (JArray)doc["author_key"];
                    }

                    if (authorKeys.Count > 0)
                    {
                        authorKey = authorKeys[0].ToString();
                    }

                    //retrieve the key used to identify books
                    string bookKey = doc["key"].ToString();
                    
                    //creates BookModel objects from the received data and adds it to the list
                    searchResults.Add(new BookModel
                    {
                        Title = doc.title,
                        Author = string.Join(", ", doc.author_name ?? new List<string>()),
                        CoverImage = $"http://covers.openlibrary.org/b/id/{doc.cover_i}-S.jpg",
                        CoverImageHigherResolution = $"http://covers.openlibrary.org/b/id/{doc.cover_i}-L.jpg",
                        PublicationYear = doc["first_publish_year"]?.ToObject<int>() ?? 0,
                        AuthorKey = authorKey,
                        Key = bookKey
                    }) ;
                }
                
            }
            return searchResults;

        }

        //gets the description for a book using its book key 
        public async  Task<string> GetDescription(string bookKey)
        {
            try
            {
                //create the URL to fetch book details
                string apiUrl = $"https://openlibrary.org{bookKey}.json";
                //send GET request to the Open Library API
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                

                //check if request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read and deserialize the JSON response
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    JObject bookObject = JObject.Parse(jsonResponse);
                    //attempts to get the description from the deserialised object
                    if (bookObject.TryGetValue("description", out JToken descriptionToken)) 
                    {
                        return descriptionToken.ToString();
                    }
                    else
                    {
                        //This will be displayed where the description would be
                        return "Description not found";
                    }

                }
                else
                {
                    Debug.WriteLine($"unsuccessful query");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        //gets the author details using the authorKey
        //method used by AuthPageViewModel to display a specific author
        public async Task<AuthorModel> GetAuthorDetailsAsync(string authorKey)
        {
            try
            {
                //url to query author's information
                string apiUrl = $"https://openlibrary.org/authors/{authorKey}.json";

                //get request sent to library
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                //Check if request was successful
                if (response.IsSuccessStatusCode)
                {
                    //read and deserialize the JSON response
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    JObject authorObject = JObject.Parse(jsonResponse);

                    //new authorModel instance
                    AuthorModel author = new AuthorModel();

                    // check if the deserialised object has a links tag - if so extract url
                    if (authorObject.ContainsKey("links"))
                    {
                        JArray linksArray = authorObject["links"] as JArray;
                        if (linksArray != null)
                        {
                            foreach (var link in linksArray)
                            {
                                JObject linkObject = link as JObject;
                                if (linkObject != null && linkObject.ContainsKey("url"))
                                {
                                    var url = linkObject["url"].ToString(); ;
                                    author.Url = url;//sets the authors url to retrieved value
                                    Debug.WriteLine($"URL found: {url}");
                                    break;//breaks since we only need one
                                }
                            }
                        }
                    }

                    //populate other properties of the AuthorModel
                    author.Name = (string)authorObject["name"];
                    author.BirthDate = (string)authorObject["birth_date"];
                    author.Bio = (string)authorObject["bio"];

                    return author;
                }
                else
                {
                    return null;
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

    }   
}
