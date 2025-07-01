using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nestify.Models;
using Nestify.Models.ViewModels;

namespace Nestify.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        private readonly MyDbContext _context;
        public HomeController(MyDbContext context)
        {
            _context = context;
        }


      
        public IActionResult Index()
        {
            var latest = _context.Properties
                .Include(l => l.SubLocation)
                .OrderByDescending(p => p.PublishDate)
                .Take(6)
                .ToList();

            var featured = _context.Properties
                .Where(p => p.IsFeatured==true)
                .Include(l => l.SubLocation)
                .OrderByDescending(p => p.PublishDate)
                //.Take(6)
                .ToList();

            ViewBag.locations = _context.Locations.ToList();


            var viewModel = new HomeViewModel
            {
                LatestProperties = latest,
                FeaturedProperties = featured
            };

            // Check if user is logged in and fetch favorite property IDs
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                int uid = int.Parse(userId);
                ViewBag.FavoriteIds = _context.FavoriteProperties
                    .Where(f => f.UserId == uid)
                    .Select(f => f.PropertyId)
                    .ToList();
            }
            else
            {
                ViewBag.FavoriteIds = new List<int>(); // prevent null exception
            }
            return View(viewModel);
        }


        //..........................Load Sublocation..........................//

        [HttpGet]
        public JsonResult GetSublocationsByLocation(int locationId)
        {
            var sublocations = _context.Sublocations
                .Where(s => s.LocationId == locationId)
                .Select(s => new { s.Id, s.SublocationName })
                .ToList();

            return Json(sublocations);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleFavorite(int propertyId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                TempData["Error"] = "Please log in to manage favorites.";
                return RedirectToAction("Index", "Home");
            }

            int uid = int.Parse(userId);
            var fav = _context.FavoriteProperties
                .FirstOrDefault(f => f.UserId == uid && f.PropertyId == propertyId);

            if (fav != null)
            {
                _context.FavoriteProperties.Remove(fav);
                TempData["Info"] = "Removed from favorites.";
            }
            else
            {
                var Newfav = new FavoriteProperty
                {
                    UserId = uid,
                    PropertyId = propertyId
                };
                _context.FavoriteProperties.Add(Newfav);

                TempData["Success"] = "Added to favorites!";
            }

            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }


        public IActionResult AboutUS()
        {
            return View();
        }


        public IActionResult LegalMatters()
        {
            return View();
        }


        

      



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
