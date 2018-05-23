using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BlindBidding.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public virtual IEnumerable<Auction> Auctions { get; set; }
        public virtual IEnumerable<Bid> Bids { get; set; }
        public virtual IEnumerable<Favourite> Favourites { get; set; }
    }
}
