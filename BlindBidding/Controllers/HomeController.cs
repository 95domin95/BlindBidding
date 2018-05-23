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

        public IActionResult HomePage()
        {
            var newAuctions = _context.Auctions.OrderByDescending(a => a.StartDate);

            var endingAuctions = _context.Auctions.OrderByDescending(a => a.EndDate);

            return View(new HomePageViewModel() {
                NewAuctions = newAuctions,
                EndingAuctions = endingAuctions
            });
        }

        public IActionResult Index(string filter="", int page=1, int onPage=10, 
            string sortingOrder="Rosnąco", string sortingExpression="Data zakończenia", string category="Samochody osobowe", string message="")
        {

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

            if(numberOfElements > 0)auctions = auctions.Skip((page-1)*onPage).Take(elToTake);

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
                NumberOfPages = numberOfPages
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
