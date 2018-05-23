using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlindBidding.Models.HomeViewModels
{
    public class IndexViewModel
    {
        public int NumberOfPages { get; set; }
        public int NumberOfElements { get; set; }
        public int Page { get; set; }
        public string Filter { get; set; }
        public string SortOrder { get; set; }
        public string SortingExpression { get; set; }
        public string Category { get; set; }
        public int OnPage { get; set; }
        public IEnumerable<Auction> Auctions { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
