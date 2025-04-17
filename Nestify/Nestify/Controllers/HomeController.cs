using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Nestify.Models;

namespace Nestify.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly MyDbContext _context;
        public HomeController(MyDbContext context)
        {
            _context = context;
        }
        

        public IActionResult Index()
        {
            var properties = _context.Properties.ToList();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult AboutUS()
        {
            return View();
        }


        public IActionResult LegalMatters()
        {
            return View();
        }


        public IActionResult Properties()
        {
            return View();
        }

        public IActionResult InteriorDesign()
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
