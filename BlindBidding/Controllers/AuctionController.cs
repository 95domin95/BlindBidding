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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BlindBidding.Controllers
{
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
        public async Task<IActionResult> AddAuction(string title = "", string description = "", string duration = "7", string category = "")
        {
            var startDate = DateTime.Now;

            var endDate = startDate.AddDays(Convert.ToDouble(duration));

            var cat = _context.Categories.Where(c => c.Name.Equals(category)).FirstOrDefault();

            var user = await _userManager.GetUserAsync(HttpContext.User);

            var auction = new Auction()
            {
                StartDate = startDate,
                EndDate = endDate,
                Description = description,
                Title = title,
                Category = cat,
                Owner = user
            };

            _context.Add(auction);

            _context.SaveChanges();

            //var user = await _userManager.GetUserAsync(HttpContext.User);

            //var cat = _context.Categories.Where(c => c.Name.Equals(category)).FirstOrDefault();

            //var auction = _context.Auctions.Where(a => a.Owner.Equals(user))
            //    .OrderByDescending(o => o.StartDate).FirstOrDefault();

            //var id = auction.AuctionId;

            //startDate = auction.StartDate;

            //var endDate = startDate.AddDays(Convert.ToDouble(duration));

            //auction.EndDate = endDate;
            //auction.Description = description;
            //auction.Title = title;
            //auction.Category = cat;
            //auction.Owner = user;

            //_context.SaveChanges();

            ViewData["AuctionAdded"] = "DodanoAukcję";

            return View(new AddAuctionViewModel()
            {
                CategoryList = _context.Categories.ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> UploadFiles([FromBody]AddAuctionFormData formData)
        {
            var category = _context.Categories.Where(c => c.Name.Equals(formData.Category)).FirstOrDefault();

            var startDate = DateTime.Now;

            var endDate = startDate.AddDays(Convert.ToDouble(formData.Duration));

            var user = await _userManager.GetUserAsync(HttpContext.User);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/auctionThumbnails/");

            var fileName = user + startDate.ToString().Replace("/", "-").Replace(" ", "-").Replace(":", "") + "_thumbnail.jpeg";

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
                    catch(Exception ex)
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

            string message = $"Files upload succesfully";

            return Json(message);
        }

        public IActionResult Index()
        {
            //List<string> names = new List<string>();
            //names.Add("Samochody osobowe");
            //names.Add("Samochody użytkowe");
            //names.Add("Motocykle i skutery");
            //names.Add("Motoryzacja");
            //names.Add("Elektronika");
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
            //        .Equals("Motoryzacja")).FirstOrDefault(),
            //        Name = i
            //    });
            //    _context.SaveChanges();
            //}

            return View();
        }
    }
}