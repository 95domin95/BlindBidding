﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BlindBidding.Data;
using BlindBidding.Models;
using BlindBidding.Models.AuctionViewModels;
using BlindBidding.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace BlindBidding.Controllers
{
    public class AddAuctionResponse
    {
        public string Message { get; set; }
    }
    public class AddAuctionFormData
    {
        public string Duration { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Data { get; set; }
    }
    public class AuctionController : Controller
    {
        private IHostingEnvironment _hostingEnv;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly UrlEncoder _urlEncoder;
        public AuctionController(
          ApplicationDbContext context,
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          IEmailSender emailSender,
          ILogger<ManageController> logger,
          UrlEncoder urlEncoder,
          IHostingEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _urlEncoder = urlEncoder;
            _hostingEnv = env;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public IActionResult AddAuction()
        {
            return View(new AddAuctionViewModel()
            {
                CategoryList = _context.Categories.ToList()
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddAuction([FromBody]AddAuctionFormData formData)
        {
            string message = String.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var category = _context.Categories.Where(c => c.Name.Equals(formData.Category))
                    .FirstOrDefault();

                var startDate = DateTime.Now;

                var endDate = startDate.AddDays(Convert.ToDouble(formData.Duration));

                var user = await _userManager.GetUserAsync(HttpContext.User);

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/auctionThumbnails/");

                var fileName = user.UserName + startDate.ToString().Replace("/", "-").Replace(" ", "-").Replace(":", "") + "_thumbnail.jpeg";

                string fileNameWitPath = "wwwroot/images/auctionThumbnails/" + fileName;
                using (FileStream fs = new FileStream(fileNameWitPath, FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        try
                        {
                            string result = Regex.Replace(formData.Data, "^data:image\\/[a-zA-Z]+;base64,", String.Empty);
                            byte[] data = Convert.FromBase64String(result);
                            bw.Write(data);
                            bw.Close();
                        }
                        catch (Exception ex)
                        {
                            string tmp = ex.Message;
                        }

                    }
                }

                var auction = new Auction()
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Description = formData.Description,
                    Title = formData.Title,
                    Category = category,
                    Owner = user,
                    ThumbnailPath = fileName
                };

                _context.Add(auction);

                _context.SaveChanges();

                message = "Pomyślnie dodano aukcję";

            }
            else
            {
                message = "Dostęp jedynie dla zalogowanych użytkowników";               
            }

            var response = new AddAuctionResponse()
            {
                Message = message
            };

            ViewData["AuctionMessage"] = message;

            return new JsonResult(response);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> RemoveFromHighlighted(int AuctionId)
        {
            var auction = _context.Auctions.Where(a => a.AuctionId.Equals(AuctionId)).FirstOrDefault();

            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user.HasHighlightedAuction)
            {
                user.HasHighlightedAuction = false;
                _context.SaveChanges();

                auction.IsHighlighted = false;
                _context.SaveChanges();
            }

            string message = "Usunięto z wyróżnionych";

            return RedirectToAction("ManageAuctions", new { message });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AddToHighlighted(int AuctionId)
        {
            var auction = _context.Auctions.Where(a => a.AuctionId.Equals(AuctionId)).FirstOrDefault();

            var user = await _userManager.GetUserAsync(HttpContext.User);

            if(!user.HasHighlightedAuction)
            {
                user.HasHighlightedAuction = true;
                _context.SaveChanges();

                auction.IsHighlighted = true;
                _context.SaveChanges();
            }

            string message = "Wyróżniono aukcję(pozostało 0 wyróżnień, usuń wyróżnienie by dodać kolejne)";

            return RedirectToAction("ManageAuctions", new { message });
        }

        [Authorize]
        public async Task<IActionResult> ManageAuctions(string viewType="My", string filter = "",
            int page = 1, int onPage = 10, string sortingOrder = "Rosnąco", string sortingExpression = "Data zakończenia",
            string category = "RTV/AGD", string ended="show", string message="")
        {
            var categories = from Categories in _context.Categories select Categories;

            var user = await _userManager.GetUserAsync(HttpContext.User);

            IQueryable<Auction> auctions = _context.Auctions.Where(a => a.Category.Name.Equals(category));

            switch (viewType)
            {
                case "My":
                    auctions = _context.Auctions.Where(a => a.Category.Name.Equals(category) && a.Owner.Equals(user));
                    break;
                case "Auctioned":
                    auctions = _context.Auctions.Where(a => a.Category.Name.Equals(category) && a.Bid != null && a.Bid.User.Equals(user));
                    break;
                case "Favourite":
                    auctions = from p in _context.Favourites
                                   join o in _context.Auctions on p.AuctionId equals o.AuctionId
                                   where p.UserId.Equals(user.Id)&&p.IsFavourite
                                   select o;
                    break;
                default:
                    auctions = auctions.Where(a => a.Owner.Equals(user));
                    break;
            }

            if (ended.Equals("hide")) auctions = auctions.Where(a => a.IsEnded.Equals(false));

            if (!string.IsNullOrWhiteSpace(filter))
            {
                auctions = auctions.Where(a => a.Title.Contains(filter));
            }

            var numberOfElements = auctions.Count();

            var numberOfPages = (int)(auctions.Count() / onPage);

            var elToTake = onPage;

            if ((auctions.Count() % onPage) != 0) numberOfPages++;

            if (page < 1) page = 1;
            if (page > numberOfPages) page = numberOfPages;

            if (page.Equals(numberOfPages)) elToTake = auctions.Count() - ((numberOfPages - 1) * onPage);

            var favouriteAuctions = from p in _context.Favourites
                                    join o in _context.Auctions on p.AuctionId equals o.AuctionId
                                    select o;

            var favourites = _context.Favourites.Where(f => f.User.Equals(user));

            List<Auction> tmp = new List<Auction>();

            if (numberOfElements>0)
            {
                auctions = auctions.Skip((page - 1) * onPage).Take(elToTake);

                if (user != null && favourites.Count() > 0)
                {
                    foreach (var i in auctions)
                    {
                        foreach (var j in favourites)
                        {
                            if (i.Equals(j.Auction) && j.IsFavourite) tmp.Add(i);
                        }
                    }
                }
            }

            switch (sortingOrder)
            {
                case "Rosnąco":
                    if (sortingExpression == "Data zakończenia") auctions = auctions.OrderBy(a => a.EndDate);
                    else auctions = auctions.OrderBy(a => a.StartDate);
                    break;
                case "Malejąco":
                    if (sortingExpression == "Data zakończenia") auctions = auctions.OrderByDescending(a => a.EndDate);
                    else auctions = auctions.OrderByDescending(a => a.StartDate);
                    break;
                default:
                    break;
            }

            ViewData["message"] = message;

            return View(new ManageAuctionsViewModel()
            {
                OnPage = onPage,
                SortOrder = sortingOrder,
                SortingExpression = sortingExpression,
                Filter = filter,
                Category = category,
                Categories = categories,
                Auctions = auctions,
                Page = page,
                NumberOfElements = numberOfElements,
                NumberOfPages = numberOfPages,
                IsAuctionAuctionedView = viewType,
                IsElementsHidden = ended.Equals("hide"),
                LogedUser = user,
                UserFavouriteAuctions = tmp,
                Favourites = favourites
            });
        }

        [HttpGet]
        [Authorize]
        public IActionResult EndAuction(int AuctionId)
        {
            var auction = _context.Auctions.Where(a => a.AuctionId.Equals(AuctionId)).FirstOrDefault();

            auction.IsEnded = true;

            _context.SaveChanges();

            string message = "Zakończono aukcję";

            return RedirectToAction("ManageAuctions", new { message });
        }

        [HttpGet]
        [Authorize]
        public IActionResult DeleteAuction(int AuctionId)
        {
            var auction = _context.Auctions.Where(a => a.AuctionId.Equals(AuctionId)).FirstOrDefault();

            if(auction.IsHighlighted)
            {
                var user = _context.Users.Where(u => u.Id.Equals(auction.OwnerId)).FirstOrDefault();

                user.HasHighlightedAuction = false;

                _context.SaveChanges();
            }

            _context.Auctions.Remove(auction);

            _context.SaveChanges();

            string message = "Usunięto aukcję";

            return RedirectToAction("ManageAuctions", new { message });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AddToFavourites(int AuctionId)
        {
            var auction = _context.Auctions.Where(a => a.AuctionId.Equals(AuctionId)).FirstOrDefault();

            if (auction != null)
            {
                var favourite = _context.Favourites.Where(f => f.Auction.Equals(auction)).FirstOrDefault();
                if(favourite!=null)
                {
                    favourite.IsFavourite = true;
                }
                else
                {
                    var favoutite = new Favourite()
                    {
                        Auction = auction,
                        User = await _userManager.GetUserAsync(HttpContext.User),
                        IsFavourite = true
                    };

                    _context.Favourites.Add(favoutite);
                }
                _context.SaveChanges();
            }

            string message = "Dodano do obserwowanych";

            return RedirectToAction("ManageAuctions", new { message });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DeleteFromFavourites(int AuctionId)
        {
            var auction = _context.Auctions.Where(a => a.AuctionId.Equals(AuctionId)).FirstOrDefault();

            if(auction!=null)
            {
                var favourite = _context.Favourites.Where(f => f.Auction.Equals(auction)).FirstOrDefault();
                favourite.IsFavourite = false;
                _context.SaveChanges();
            }

            string message = "Usunięto z obserwowanych";

            return RedirectToAction("ManageAuctions", new { message });
        }

        [HttpGet]
        public async Task<IActionResult> AuctionView(int id, string message="")
        {
            var auction = _context.Auctions.Where(a => a.AuctionId
            .Equals(id)).FirstOrDefault();

            bool isBidable = true;

            bool isSold = false;

            double userActualBid = 0.0;

            bool isLogged = User.Identity.IsAuthenticated;

            var user = await _userManager.GetUserAsync(HttpContext.User);

            bool isAuctionOwner = false;

            if (user != null)
            {
                isAuctionOwner = user.Equals(auction.Owner);

                var tmp = _context.Bids.Where(b => b.Auction.Equals(auction)
                && b.User.Equals(user)).OrderByDescending(b => b.BidPrice);

                if(tmp.Any())
                {
                    userActualBid = tmp.First().BidPrice;
                }
            }

            Bid winningBid = default(Bid);
            ApplicationUser winner = default(ApplicationUser);

            if (!isLogged || isAuctionOwner)
            {
                isBidable = false;
            }
            if(isLogged&&auction.IsEnded)
            {
                if(auction.BidId != null)
                {
                    isSold = true;

                    winningBid = _context.Bids.Where(b => b.Auction.Equals(auction)).FirstOrDefault();

                    winner = _context.Users.Where(u => u.Id.Equals(winningBid.UserId)).FirstOrDefault();
                }
            }

            var owner = _context.Users.Where(u => u.Id.Equals(auction.OwnerId)).FirstOrDefault();

            TimeSpan remains = auction.EndDate - DateTime.Now;

            ViewData["BidAdded"] = message;

            var toAuctionEnd = ((int)remains.TotalDays).ToString() + " dni";

            if (remains.TotalDays < 1)
            {
                toAuctionEnd = string.Format("{0:hh\\:mm\\:ss}", remains);
            }

            return View(new AuctionViewModel()
            {
                Remains = toAuctionEnd,
                Auction = auction,
                IsBidable = isBidable,
                IsSold = isSold,
                IsAuctionOwner = isAuctionOwner,
                UserActualBid = userActualBid,
                Owner = owner,
                Winner = winner,
                WinningBid = winningBid
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddBidAsync(int id, int bidValue)
        {
            string message = String.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var auction = _context.Auctions.Where(a => a.AuctionId.Equals(id))
                .FirstOrDefault();

                var bid = new Bid()
                {
                    BidPrice = bidValue,
                    Auction = auction,
                    User = await _userManager.GetUserAsync(HttpContext.User)
                };

                _context.Bids.Add(bid);
                _context.SaveChanges();

                if (auction.Bid != null)
                {
                    if (auction.Bid.BidPrice < bidValue)
                    {
                        auction.Bid = bid;
                        _context.SaveChanges();
                    }
                }
                else
                {
                    auction.Bid = bid;
                    _context.SaveChanges();
                }
                _context.SaveChanges();

                message = "Dodano ofertę";
            }
            else
            {
                message = "Zaloguj się by licytować";
            }

            return RedirectToAction("AuctionView", new {id, message });
        }
        public IActionResult Index()
        {
            List<string> names = new List<string>();
            //names.Add("Samochody osobowe");
            //names.Add("Samochody użytkowe");
            //names.Add("Motocykle i skutery");
            //names.Add("Motoryzacja");
            //names.Add("Elektronika");
            //names.Add("RTV/AGD");
            //names.Add("Komputrey stacjonarne");
            //names.Add("Telefony i smartfony");
            //names.Add("Części komputrowe");
            //names.Add("Słuchawki");
            //names.Add("Głośniki");
            //names.Add("Telewizory i monitory");

            //names.Add("Nieruchomości");
            //names.Add("Sport i Hobby");
            //names.Add("Rolnictwo");
            //names.Add("Muzyka");
            //names.Add("Edukacja");
            //names.Add("Dom i Ogród");

            //foreach (var i in names)
            //{
            //    _context.Categories.Add(new Category()
            //    {
            //        SubcategoryOf = null,
            //        Name = i,
            //    });
            //    _context.SaveChanges();
            //}

            //foreach (var i in names)
            //{
            //    _context.Categories.Add(new Category()
            //    {
            //        SubcategoryOf = _context.Categories.Where(c => c.Name
            //        .Equals("Elektronika")).FirstOrDefault(),
            //        Name = i
            //    });
            //    _context.SaveChanges();
            //}

            return View();
        }
    }
}