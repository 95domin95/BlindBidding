using BlindBidding.Models.HomeViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlindBidding.Models.AuctionViewModels
{
    public class ManageAuctionsViewModel : IndexViewModel
    {
        public string IsAuctionAuctionedView { get; set; }

        public bool IsElementsHidden { get; set; }
    }
}
