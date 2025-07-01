using Microsoft.AspNetCore.Mvc;
using Nestify.Models.ViewModels;
using Nestify.Models;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;

namespace Nestify.Controllers
{
    public class InteriorDesignController : Controller
    {
        private readonly MyDbContext _context;
        private readonly SmtpSettings _smtpSettings;
        private readonly IWebHostEnvironment _env;

        public InteriorDesignController(MyDbContext context, IOptions<SmtpSettings> smtpSettings, IWebHostEnvironment env)
        {
            _context = context;
            _smtpSettings = smtpSettings.Value;
            _env = env;
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

            try
            {
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

                var mail = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.UserName),
                    Subject = $"New Interior Design Request from {model.Name}",
                    Body = $"Name: {model.Name}\nEmail: {model.Email}\nPhone: {model.PhoneNumber}\n\nMessage:\n{model.Message}",
                    IsBodyHtml = false
                };
                mail.To.Add("nestifyre@gmail.com");

                var smtp = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
                {
                    Credentials = new NetworkCredential(_smtpSettings.UserName, _smtpSettings.Password),
                    EnableSsl = _smtpSettings.EnableSsl
                };
                smtp.Send(mail);

                TempData["Success"] = "Your request has been sent successfully!";
            }
            catch
            {
                TempData["Error"] = "Something went wrong while sending your request.";
            }

            return RedirectToAction(nameof(interiorDesign));
        }

        public IActionResult Gallery()
        {
            var items = _context.InteriorGalleries.ToList();
            return View(items);
        }
    
    }
}
