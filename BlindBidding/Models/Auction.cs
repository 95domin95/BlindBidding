using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BlindBidding.Models
{
    public class Auction
    {
        public bool IsHighlighted { get; set; }
        public bool IsEnded { get; set; }
        public int AuctionId { get; set; }
        [StringLength(70)]
        public string Title { get; set; }
        [StringLength(5000)]
        public string Description { get; set; }
        [Required]
        public int AllUsrAuctionsCount { get; set; }
        public string ThumbnailPath { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public string OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public ApplicationUser Owner { get; set; }

        public int? BidId { get; set; }
        [ForeignKey("BidId")]
        public virtual Bid Bid { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
