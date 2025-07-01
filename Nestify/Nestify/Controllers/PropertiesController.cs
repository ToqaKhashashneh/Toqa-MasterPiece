using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nestify.Models;

namespace Nestify.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly MyDbContext _context;

        public PropertiesController(MyDbContext context)
        {
            _context = context;
        }



        //............................................Filter and Sort Properties................................

        public IActionResult Properties(
                string search,
                string sort,
                string PropertyType = null,
                int? LocationId = null,
                int? SubLocationId = null,
                int? Bedrooms = null,
                decimal? MinSize = null,
                decimal? MaxSize = null,
                decimal? MaxPrice = null)
        {
            var query = _context.Properties
                .Include(p => p.SubLocation)
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.PropertyName.Contains(search));

            // Filters
            if (!string.IsNullOrEmpty(PropertyType))
                query = query.Where(p => p.PropertyType == PropertyType);

            if (LocationId.HasValue)
                query = query.Where(p => p.SubLocation.LocationId == LocationId);

            if (SubLocationId.HasValue)
                query = query.Where(p => p.SubLocationId == SubLocationId);

            if (Bedrooms.HasValue)
                query = query.Where(p => p.Bedrooms == Bedrooms);

            if (MinSize.HasValue)
                query = query.Where(p => p.Size >= MinSize);

            if (MaxSize.HasValue)
                query = query.Where(p => p.Size <= MaxSize);

            if (MaxPrice.HasValue)
                query = query.Where(p => p.Price <= MaxPrice);

            // Sorting
            //switch (sort)
            //{
            //    case "priceLow": query = query.OrderBy(p => p.Price); break;
            //    case "priceHigh": query = query.OrderByDescending(p => p.Price); break;
            //    case "new": query = query.OrderByDescending(p => p.PublishDate); break;
            //    default: query = query.OrderByDescending(p => p.Id); break;
            //}
            query = sort switch
            {
                "priceLow" => query.OrderBy(p => p.Price),
                "priceHigh" => query.OrderByDescending(p => p.Price),
                "new" => query.OrderByDescending(p => p.PublishDate),
                _ => query.OrderByDescending(p => p.Id)
            };

            // Return all (no pagination)
            var properties = query.ToList();

            // Optional ViewBag filters (for keeping selections)
            ViewBag.Search = search;
            ViewBag.Sort = sort;
            ViewBag.PropertyType = PropertyType;
            ViewBag.LocationId = LocationId;
            ViewBag.SubLocationId = SubLocationId;
            ViewBag.Bedrooms = Bedrooms;
            ViewBag.MinSize = MinSize;
            ViewBag.MaxSize = MaxSize;
            ViewBag.MaxPrice = MaxPrice;


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

            return View(properties);
        }



        public IActionResult PropertyDetails(int id)
        {

            var property = _context.Properties

           .Include(p => p.PropertyFeatures)
           .FirstOrDefault(p => p.Id == id);

            if (property == null) return NotFound();

            return View(property);
        }


        [Authorize]
        [HttpPost]
        public IActionResult ScheduleVisit(PropertyInquiry model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("PropertyDetails", new { id = model.PropertyId });
            }

            model.UserId = user.Id;
            model.Name = user.FirstName;
            model.Email = user.Email;
            model.Phone = user.PhoneNumber;
            model.SubmittedAt = DateTime.Now;
            model.Status = "Pending";




            if (model.PreferredDate == null || model.Message == null)
            {
                TempData["Error"] = "All fields are required.";
                return RedirectToAction("PropertyDetails", new { id = model.PropertyId });
            }
            

            _context.PropertyInquiries.Add(model);
            _context.SaveChanges();

            TempData["Success"] = "Visit scheduled successfully!";
            return RedirectToAction("PropertyDetails", new { id = model.PropertyId });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleFavorite(int propertyId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                TempData["Error"] = "Please log in to manage favorites.";
                return RedirectToAction("Properties", "Properties");
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
            return RedirectToAction("Properties", "Properties");
        }



    }




}
