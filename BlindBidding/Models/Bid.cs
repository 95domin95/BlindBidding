using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BlindBidding.Models
{
    public class Bid
    {
        public int BidId { get; set; }
        public double BidPrice { get; set; }

        public int AuctionId { get; set; }
        [ForeignKey("AuctionId")]
        public Auction Auction { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
