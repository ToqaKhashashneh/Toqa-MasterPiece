using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nestify.Models;


namespace Nestify.Controllers
{
    public class AdminController : Controller
    {
        private readonly MyDbContext _context;

        public AdminController(MyDbContext context)
        {
            _context = context;
        }

        // Dashboard
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Dashboard()
        {
            ViewBag.PropertiesCount = await _context.Properties.CountAsync();
            ViewBag.UsersCount = await _context.Users.CountAsync();
            ViewBag.PropertyInquiriesCount = await _context.PropertyInquiries.CountAsync();
            ViewBag.ContactInquiriesCount = await _context.ContactInquiries.CountAsync();
            
            // Get recent property inquiries
            ViewBag.RecentPropertyInquiries = await _context.PropertyInquiries
                .Include(p => p.Property)
                .OrderByDescending(p => p.SubmittedAt)
                .Take(5)
                .ToListAsync();
                
            // Get recent contact inquiries
            ViewBag.RecentContactInquiries = await _context.ContactInquiries
                .OrderByDescending(c => c.SubmittedAt)
                .Take(5)
                .ToListAsync();
                
            // Get recent properties
            ViewBag.RecentProperties = await _context.Properties
                .OrderByDescending(p => p.PublishDate)
                .Take(5)
                .ToListAsync();
                
            return View();
        }

        // All Properties
        public async Task<IActionResult> Properties()
        {
            var properties = await _context.Properties
                .Include(p => p.User)
                .Include(p => p.SubLocation)
                .ThenInclude(s => s.Location)
                .OrderByDescending(p => p.PublishDate)
                .ToListAsync();
                
            return View(properties);
        }
        
        // View Property Details
        public async Task<IActionResult> PropertyDetails(int id)
        {
            var property = await _context.Properties
                .Include(p => p.User)
                .Include(p => p.SubLocation)
                .ThenInclude(s => s.Location)
                .Include(p => p.PropertyFeatures)
                .FirstOrDefaultAsync(p => p.Id == id);
                
            if (property == null)
            {
                return NotFound();
            }
            
            return View(property);
        }
        
        // Users
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users
                .OrderByDescending(u => u.Id)
                .ToListAsync();
                
            return View(users);
        }
        
        // Verify User
        [HttpPost]
        public async Task<IActionResult> VerifyUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }
            
            user.IsVerifiedBusiness = true;
            await _context.SaveChangesAsync();
            
            return Json(new { success = true });
        }
        
        // Unverify User
        [HttpPost]
        public async Task<IActionResult> UnverifyUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }
            
            user.IsVerifiedBusiness = false;
            await _context.SaveChangesAsync();
            
            return Json(new { success = true });
        }
        
        // Payments
        public async Task<IActionResult> Payments()
        {
            var payments = await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Package)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
                
            return View(payments);
        }

        // Complete Payment
        [HttpPost]
        public async Task<IActionResult> CompletePayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                TempData["Error"] = "Payment not found.";
                return RedirectToAction("Payments"); // Adjust the action name if needed
            }

            if (payment.Status == "Pending")
            {
                payment.Status = "Completed";
                await _context.SaveChangesAsync();
                TempData["Success"] = "Payment marked as completed.";
            }
            else
            {
                TempData["Error"] = "Payment is not in Pending status.";
            }

            return RedirectToAction("Payments"); // Adjust the action name if needed
        }

        // View Packages
        public async Task<IActionResult> Packages()
        {
            var packages = await _context.Packages.OrderBy(p => p.Price).ToListAsync();
            return View(packages);
        }

        // Save Package (Handles both Add and Edit)
        [HttpPost]
        public async Task<IActionResult> SavePackage(Package package)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Packages");

            if (package.Id == 0)
            {
                _context.Packages.Add(package);
                TempData["Success"] = "Package added successfully.";
            }
            else
            {
                var existingPackage = await _context.Packages.FindAsync(package.Id);
                if (existingPackage != null)
                {
                    existingPackage.Name = package.Name;
                    existingPackage.Price = package.Price;
                    existingPackage.PostLimit = package.PostLimit;
                    existingPackage.DurationInDays = package.DurationInDays;
                    TempData["Success"] = "Package updated successfully.";
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Packages");
        }

        // Delete Package
        [HttpPost]
        public async Task<IActionResult> DeletePackage(int id)
        {
            var package = await _context.Packages.FindAsync(id);
            if (package != null)
            {
                _context.Packages.Remove(package);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Package deleted successfully.";
            }
            return RedirectToAction("Packages");
        }

        //..............................................................PropertyInquiries..........................................................  
        // Property Inquiries
        public async Task<IActionResult> PropertyInquiries()
        {
            var inquiries = await _context.PropertyInquiries
                .Include(p => p.Property)
                .Include(p => p.User)
                .OrderByDescending(p => p.SubmittedAt)
                .ToListAsync();
                
            return View(inquiries);
        }
        
        // Update Property Inquiry Status
        [HttpPost]
        public async Task<IActionResult> UpdateInquiryStatus(int id, string status)
        {
            var inquiry = await _context.PropertyInquiries.FindAsync(id);
            if (inquiry == null)
            {
                TempData["Error"] = "Inquiry not found.";
                return RedirectToAction("PropertyInquiries");
            }
            
            inquiry.Status = status;
            await _context.SaveChangesAsync();
            
           
            return RedirectToAction("PropertyInquiries");
        }

        // Delete Property Inquiry
        [HttpPost]
        public async Task<IActionResult> DeletePropertyInquiry(int id)
        {
            var inquiry = await _context.PropertyInquiries.FindAsync(id);
            if (inquiry == null)
            {
                TempData["Error"] = "Property inquiry not found.";
                return RedirectToAction("PropertyInquiries");
            }

            _context.PropertyInquiries.Remove(inquiry);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Property inquiry deleted successfully.";
            return RedirectToAction("PropertyInquiries");
        }


        //..............................................................InteriorDesignInquiries..........................................................


        // Interior Design Inquiries
        public async Task<IActionResult> InteriorDesign()
        {
            var inquiries = await _context.InteriorDesignInquiries
                .OrderByDescending(i => i.SubmittedAt)
                .ToListAsync();
                
            return View(inquiries);
        }




        //...............................................................ContactInquiries..............................................................
        // Contact Inquiries
        public async Task<IActionResult> ContactInquiries()
        {
            var inquiries = await _context.ContactInquiries
                .OrderByDescending(c => c.SubmittedAt)
                .ToListAsync();
                
            return View(inquiries);
        }



        // Delete Contact Inquiry
        [HttpPost]
        public async Task<IActionResult> DeleteContactInquiry(int id)
        {
            var inquiry = await _context.ContactInquiries.FindAsync(id);
            if (inquiry == null)
            {
                TempData["Error"] = "Inquiry not found.";
                return RedirectToAction("ContactInquiries");
            }

            _context.ContactInquiries.Remove(inquiry);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Inquiry deleted successfully.";
            return RedirectToAction("ContactInquiries");
        }



        //..............................................................Locations and Sublocations..........................................................

        // Locations
        public async Task<IActionResult> Locations()
        {
            var locations = await _context.Locations
                .Include(l => l.Sublocations)
                .ToListAsync();
                
            return View(locations);
        }

        // Save Location (Handles both Add and Edit)
        [HttpPost]
        public async Task<IActionResult> SaveLocation(Location location)
        {
            if (ModelState.IsValid)
            {
                if (location.Id == 0)
                {
                    _context.Locations.Add(location); // Add new
                }
                else
                {
                    var existing = await _context.Locations.FindAsync(location.Id);
                    if (existing == null)
                        return NotFound();

                    existing.LocationName = location.LocationName; // Update existing
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Locations));
            }

            // If model invalid
            var locations = await _context.Locations.Include(l => l.Sublocations).ToListAsync();
            return View("Locations", locations);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            var location = await _context.Locations
                .Include(l => l.Sublocations) 
                .FirstOrDefaultAsync(l => l.Id == id);

            if (location == null)
            {
                TempData["Error"] = "Location not found.";
                return RedirectToAction("Locations");
            }

            // First delete all related sublocations
            if (location.Sublocations != null && location.Sublocations.Any())
            {
                _context.Sublocations.RemoveRange(location.Sublocations);
            }

            // Then delete the location itself
            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Location and all related Sublocations deleted successfully!";
            return RedirectToAction("Locations");
        }


        // Sublocations list
        public async Task<IActionResult> Sublocations()
        {
            var sublocations = await _context.Sublocations
                .Include(s => s.Location)
                .ToListAsync();

            ViewBag.Locations = await _context.Locations.ToListAsync();

            return View(sublocations);
        }


        [HttpPost]
        public async Task<IActionResult> SaveSublocation(Sublocation model)
        {
            if (model.Id == 0)
            {
                _context.Sublocations.Add(model);
                TempData["Success"] = "Sublocation added successfully.";
            }
            else
            {
                var existing = await _context.Sublocations.FindAsync(model.Id);
                if (existing != null)
                {
                    existing.SublocationName = model.SublocationName;
                    existing.LocationId = model.LocationId;
                    TempData["Success"] = "Sublocation updated successfully.";
                }
                else
                {
                    TempData["Error"] = "Sublocation not found.";
                    return RedirectToAction(nameof(Sublocations));
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Sublocations));
        }

        // Delete Sublocation
        [HttpPost]
        public async Task<IActionResult> DeleteSublocation(int id)
        {
            var sublocation = await _context.Sublocations.FindAsync(id);
            if (sublocation != null)
            {
                _context.Sublocations.Remove(sublocation);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Sublocation deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Sublocation not found.";
            }
            return RedirectToAction(nameof(Sublocations));
        }


        // Toggle Featured Status
        [HttpPost]
        public async Task<IActionResult> ToggleFeatured(int id, bool isFeatured)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                TempData["SweetError"] = "Property not found.";
             return   RedirectToAction("PropertyDetails", new { id });
            }

            property.IsFeatured = isFeatured;
            await _context.SaveChangesAsync();

            TempData["SweetSuccess"] = isFeatured ? "Property featured successfully." : "Property unfeatured successfully.";
            return RedirectToAction("PropertyDetails", new { id });
        }




        // Delete Property
        [HttpPost]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            var property = await _context.Properties
                .Include(p => p.PropertyFeatures)
                .Include(p => p.PropertyInquiries)
                .Include(p => p.FavoriteProperties)
                .FirstOrDefaultAsync(p => p.Id == id);
                
            if (property == null)
            {
                TempData["SweetError"] = "Property not found.";
                return RedirectToAction("Properties");
            }
            
            // Remove related entities first
            _context.PropertyFeatures.RemoveRange(property.PropertyFeatures);
            _context.PropertyInquiries.RemoveRange(property.PropertyInquiries);
            _context.FavoriteProperties.RemoveRange(property.FavoriteProperties);
            
            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();

            return RedirectToAction("Properties");
        }

        
      












    }
}