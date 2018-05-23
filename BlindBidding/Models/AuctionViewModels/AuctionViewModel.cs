using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlindBidding.Models.AuctionViewModels
{
    public class AuctionViewModel
    {
        public Bid WinningBid { get; set; }
        public ApplicationUser Winner { get; set; }
        public double UserActualBid { get; set; }
        public bool IsAuctionOwner { get; set; }
        public bool IsSold { get; set; }
        public bool IsBidable { get; set; }
        public string Remains { get; set; }
        public Auction Auction { get; set; }
        public ApplicationUser Owner { get; set; }
    }
}
