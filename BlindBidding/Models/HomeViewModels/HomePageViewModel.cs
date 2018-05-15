using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlindBidding.Models.HomeViewModels
{
    public class HomePageViewModel
    {
        public IEnumerable<Auction> NewAuctions { get; set; }
        public IEnumerable<Auction> EndingAuctions { get; set; }
    }
}
