using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Nestify.Models;
using Nestify.Models.ViewModels;
using Microsoft.Extensions.Options;


namespace Nestify.Controllers
{
    public class ContactUsController : Controller
    {
        private readonly MyDbContext _context;

        private readonly SmtpSettings _smtpSettings;

        public ContactUsController(MyDbContext context, IOptions<SmtpSettings> smtpSettings)
        {
            _context = context;
            _smtpSettings = smtpSettings.Value;
        }

        public IActionResult Contact()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(ContactFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all required fields.";
                return RedirectToAction("Contact");
            }

            try
            {
                // Save to database
                var inquiry = new ContactInquiry
                {
                    Name = model.Name,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Message = model.Message,
                    SubmittedAt = DateTime.Now
                };
                _context.ContactInquiries.Add(inquiry);
                _context.SaveChanges();

                // Send email
                var mail = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.UserName),
                    Subject = $"New Contact Message from {model.Name}",
                    Body = $"Name: {model.Name}\nEmail: {model.Email}\nPhone: {model.PhoneNumber}\n\n{model.Message}",
                    IsBodyHtml = false
                };
                mail.To.Add("nestifyre@gmail.com");

                var smtp = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
                {
                    Credentials = new NetworkCredential(_smtpSettings.UserName, _smtpSettings.Password),
                    EnableSsl = _smtpSettings.EnableSsl
                };
                smtp.Send(mail);

                TempData["Success"] = "Your message has been sent successfully!";
            }
            catch
            {
                TempData["Error"] = "Something went wrong while sending your message.";
            }

            return RedirectToAction("Contact");
        }



    }
}

