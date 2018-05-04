using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlindBidding.Models
{
    public class Bid
    {
        public int BidId { get; set; }
        public double BidPrice { get; set; }
        public Auction Auction { get; set; }
        public ApplicationUser User { get; set; }
    }
}
