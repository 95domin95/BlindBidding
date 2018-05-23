using System;
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

        public IActionResult DeleteAuction()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddAuction([FromBody]AddAuctionFormData formData)
        {
            string message = String.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var category = _context.Categories.Where(c => c.Name.Equals(formData.Category)).FirstOrDefault();

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

        [Authorize]
        public async Task<IActionResult> ManageAuctions(string viewType="My", string filter = "", int page = 1, int onPage = 10,
           string sortingOrder = "Rosnąco", string sortingExpression = "Data zakończenia", string category = "Samochody osobowe", string ended="show")
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
                default:
                    auctions = auctions.Where(a => a.Owner.Equals(user));
                    break;
            }

            if (ended.Equals("hide")) auctions = auctions.Where(a => a.IsEnded.Equals(true));

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

            if(numberOfElements>0)auctions = auctions.Skip((page - 1) * onPage).Take(elToTake);

            switch (sortingOrder)
            {
                case "Rosnąco":
                    auctions = auctions.OrderBy(a => a.EndDate);
                    break;
                case "Malejąco":
                    auctions = auctions.OrderByDescending(a => a.EndDate);
                    break;
                default:
                    break;
            }

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
                IsAuctionAuctionedView = viewType.Equals("Auctioned"),
                IsElementsHidden = ended.Equals("hide")
            });
        }
        [HttpGet]
        public IActionResult EndAuction(int AuctionId)
        {
            var auction = _context.Auctions.Where(a => a.AuctionId.Equals(AuctionId)).FirstOrDefault();

            auction.IsEnded = true;

            _context.SaveChanges();

            return RedirectToAction("ManageAuctions");
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