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

namespace BlindBidding.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = from Categories in _context.Categories select Categories;

            var auctions = from Auctions in _context.Auctions select Auctions;

            return View(new IndexViewModel()
            {
                OnPage = 1,
                SortOrder = "Descending",
                SortingExpression = "Data zakończenia",
                Filter = "",
                Category = "Samochody osobowe",
                Categories = categories,
                Auctions = auctions
            });
        }

        public IActionResult HomePage()
        {
            var newAuctions = _context.Auctions.OrderByDescending(a => a.StartDate);

            var endingAuctions = _context.Auctions.OrderByDescending(a => a.EndDate);

            return View(new HomePageViewModel() {
                NewAuctions = newAuctions,
                EndingAuctions = endingAuctions
            });
        }

        [HttpGet]
        public IActionResult Index(string filter="", int page=1, int onPage=10, 
            string sortingOrder="Rosnąco", string sortingExpression="Data zakończenia", string category="Samochody osobowe")
        {
            var categories = from Categories in _context.Categories select Categories;

            var auctions = _context.Auctions.Where(a => a.Title.Contains(filter)
            && a.Category.Name.Equals(category));

            switch(sortingOrder)
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

            return View(new IndexViewModel()
            {
                OnPage = onPage,
                SortOrder = sortingOrder,
                SortingExpression = sortingExpression,
                Filter = filter,
                Category = category,
                Categories = categories,
                Auctions = auctions
            });
        }

        //public async Task<IActionResult> Index(string filter="", int page=1, string sortExpression = "", 
        //    string category="", int elementsOnPage=10)
        //{
        //    var auction = _context.Auctions.Where(a => a.Category.Equals(category));

        //    if (category.Equals("")) auction = from Auction in _context.Auctions select Auction;

        //    if (auction.Count().Equals(0)) return Content("Brak zawartości do wyświetlenia.");

        //    if (!string.IsNullOrWhiteSpace(filter))
        //    {
        //        auction = auction.Where(a => a.Description.Contains(filter)||a.Title.Contains(filter));
        //    }

        //    var categories = _context.Categories.ToList();

        //    //var tmp = new List<IndexViewModel>();

        //    //tmp.Add(new IndexViewModel()
        //    //{
        //    //    Auctions = auction,
        //    //    Categories = categories
        //    //});

        //    //IQueryable<IndexViewModel> viewModel = tmp.AsQueryable();

        //    var model = await PagingList.CreateAsync(auction, elementsOnPage, page, sortExpression, "");

        //    model.RouteValue = new RouteValueDictionary {
        //        { "filter", filter},
        //        { "category", category},
        //        { "elementsOnPage", elementsOnPage}
        //    };

        //    return View(new IndexViewModel() {
        //        Categories = categories,
        //        Auctions = model
        //    });
        //}

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
