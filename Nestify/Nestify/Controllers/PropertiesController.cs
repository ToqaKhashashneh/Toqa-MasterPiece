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





        //..............................Fetch Properties from SQL.......................
        public IActionResult Properties(string search, string sort, int page = 1, int pageSize = 9)
        {

            var query = _context.Properties.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.PropertyName.Contains(search));
            }

            // Sorting
            switch (sort)
            {
                case "priceLow":
                    query = query.OrderBy(p => p.Price);
                    break;
                case "priceHigh":
                    query = query.OrderByDescending(p => p.Price);
                    break;
                case "new":
                    query = query.OrderByDescending(p => p.PublishDate);
                    break;
                default:
                    query = query.OrderByDescending(p => p.Id); // default sorting
                    break;
            }

            // Count
            ViewBag.TotalCount = query.Count();

            // Pagination
            var paginatedList = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Pass filters to ViewBag for persistence
            ViewBag.CurrentPage = page;
            ViewBag.Search = search;
            ViewBag.Sort = sort;
            ViewBag.PageSize = pageSize;

            return View(paginatedList);
         
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




    }




}
