using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlindBidding.Models
{
    public class Auction
    {
        public int AuctionId { get; set; }
        [StringLength(70)]
        public string Title { get; set; }
        [StringLength(5000)]
        public string Description { get; set; }
        public Bid AcceptedBid { get; set; }
        public ApplicationUser Owner { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}
