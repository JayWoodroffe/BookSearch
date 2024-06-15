using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookSearch_UWP_fr.ViewModels
{
    public class BookModel
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string CoverImage { get; set; }
        public string CoverImageHigherResolution { get; set; }
        
        public string ISBN { get; set; }
        public string Key{ get; set; }
        public string Description { get; set; } 
        public string AuthorKey { get; set; }

        public int PublicationYear { get; set; }

    }
}