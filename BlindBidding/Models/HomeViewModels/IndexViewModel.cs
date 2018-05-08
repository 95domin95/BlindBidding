using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlindBidding.Models.HomeViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Auction> Auctions { get; set; }
    }
}
