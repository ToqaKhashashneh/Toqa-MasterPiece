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
                TempData["Error"] = "Model is not valid!";
                return RedirectToAction("ContactUs");
            }



            // Step 1: DB Save Check
            Console.WriteLine("Step 1: Starting DB Save...");
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

            // Step 2: Email Send Check
            Console.WriteLine("Step 2: Preparing Email...");
            var mail = new MailMessage
            {
                From = new MailAddress(_smtpSettings.UserName),
                Subject = $"New Contact Message From: {model.Name}",
                Body = $"Full Name: {model.Name}\n" +
                       $"Email: {model.Email}\n" +
                       $"Phone: {model.PhoneNumber}\n\n" +
                       $"Message:\n{model.Message}",
                IsBodyHtml = false
            };

            mail.To.Add("nestifyre@gmail.com");

            var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                Credentials = new NetworkCredential(_smtpSettings.UserName, _smtpSettings.Password),
                EnableSsl = _smtpSettings.EnableSsl
            };

            smtpClient.Send(mail);
            Console.WriteLine("Step 2: Email sent ✅");

            TempData["Success"] = "Sent Successfuly!";




            TempData["Error"] = $"Something Went Wrong";


            return RedirectToAction("Contact");



        }


    }
}

