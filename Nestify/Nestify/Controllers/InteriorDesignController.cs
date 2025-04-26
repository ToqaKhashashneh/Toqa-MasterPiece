using Microsoft.AspNetCore.Mvc;
using Nestify.Models.ViewModels;
using Nestify.Models;

namespace Nestify.Controllers
{
    public class InteriorDesignController : Controller
    {
        private readonly MyDbContext _context;

        public InteriorDesignController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Show the page
        public IActionResult interiorDesign()
        {
            return View();
        }

        // POST: Submit the form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitInteriorDesign(InteriorDesignViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill out all fields correctly.";
                return RedirectToAction(nameof(interiorDesign));
            }

            var inquiry = new InteriorDesignInquiry
            {
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Message = model.Message,
                SubmittedAt = DateTime.Now
            };

            _context.InteriorDesignInquiries.Add(inquiry);
            _context.SaveChanges();

            TempData["Success"] = "Your request has been sent successfully!";
            return RedirectToAction(nameof(interiorDesign));
        }
    }
}
