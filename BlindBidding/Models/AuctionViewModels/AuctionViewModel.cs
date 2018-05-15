using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlindBidding.Models.AuctionViewModels
{
    public class AuctionViewModel
    {
        public int Remains { get; set; }
        public Auction Auction { get; set; }
    }
}
