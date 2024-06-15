using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookSearch_UWP_fr.Models
{
    public class SearchHistoryItemModel
    {
        public string SearchTerm { get; set; }
        public string SearchCriteria { get; set; }
        public DateTime Timestamp { get; set; }
    }
    
}
