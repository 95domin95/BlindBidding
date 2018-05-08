using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BlindBidding.Models;
using BlindBidding.Data;
using BlindBidding.Models.HomeViewModels;

namespace BlindBidding.Controllers
{
    public class HomeController : Controller
    {
        public ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ItemsList()
        {
            var itemsList = _context.Auctions.OrderBy(a => a.EndDate).Take(100).ToList();

            var categories = _context.Categories.ToList();

            return View(new ItemsListViewModel()
            {
                Auctions = itemsList,
                Categories = categories
            });
        }

        [HttpGet]
        public IActionResult ItemsList(string SearchString, string Category, string elementsOnPage, string sortingOrder)
        {
            var itemsList = _context.Auctions.Where(a => a.Description.Contains(SearchString)
            || a.Title.Contains(SearchString)||0==0/* && a.Category.Equals(Category)*/);

            var categories = _context.Categories.ToList();

            return View(new ItemsListViewModel() {
                Auctions = itemsList,
                Categories = categories
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
