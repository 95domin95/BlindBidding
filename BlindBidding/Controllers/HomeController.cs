using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BlindBidding.Models;
using BlindBidding.Data;
using BlindBidding.Models.HomeViewModels;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using BlindBidding.Services;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;

namespace BlindBidding.Controllers
{
    public class HomeController : Controller
    {
        private IHostingEnvironment _hostingEnv;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly UrlEncoder _urlEncoder;
        public HomeController(
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

        public IActionResult HomePage()
        {
            var newAuctions = _context.Auctions.OrderByDescending(a => a.StartDate).Take(5);

            var endingAuctions = _context.Auctions.OrderByDescending(a => a.EndDate).Take(5);

            return View(new HomePageViewModel() {
                NewAuctions = newAuctions,
                EndingAuctions = endingAuctions
            });
        }

        public async Task<IActionResult> Index(string filter="", int page=1, int onPage=10, 
            string sortingOrder="Rosnąco", string sortingExpression="Data zakończenia", string category="Samochody osobowe", string message="")
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var categories = from Categories in _context.Categories select Categories;

            var auctions = _context.Auctions.Where(a => a.Category.Name.Equals(category));

            auctions = auctions.Where(a => a.EndDate < DateTime.Now);

            foreach(var i in auctions)
            {
                i.IsEnded = true;              
            }

            _context.SaveChanges();

            auctions = _context.Auctions.Where(a => a.Category.Name.Equals(category)&&a.IsEnded.Equals(false));

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
                                    where p.UserId.Equals(user.Id)&&p.IsFavourite
                                    select o;

            var favourites = _context.Favourites.Where(f => f.User.Equals(user));

            List<Auction> tmp = new List<Auction>();

            if (numberOfElements > 0)
            {
                auctions = auctions.Skip((page - 1) * onPage).Take(elToTake);

                if(user!=null&&favourites.Count() > 0)
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
                    if(sortingExpression == "Data zakończenia") auctions = auctions.OrderBy(a => a.EndDate);
                    else auctions = auctions.OrderBy(a => a.StartDate);
                    break;
                case "Malejąco":
                    if (sortingExpression == "Data zakończenia") auctions = auctions.OrderByDescending(a => a.EndDate);
                    else auctions = auctions.OrderByDescending(a => a.StartDate);
                    break;
                default:
                    break;
            }

            if (message.Equals("Wylogowanie powiodło się.")) ViewData["Logout"] = "Wylogowanie powiodło się.";

            return View(new IndexViewModel()
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
                LogedUser = user,
                UserFavouriteAuctions = tmp,
                Favourites = favourites
            });
        }

        public IActionResult Item()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
