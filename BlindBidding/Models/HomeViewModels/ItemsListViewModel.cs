using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlindBidding.Models.HomeViewModels
{
    public class ItemsListViewModel
    {
        public IEnumerable<Auction> Auctions { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
