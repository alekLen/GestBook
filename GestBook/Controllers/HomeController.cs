using GestBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GestBook.Controllers
{
    public class HomeController : Controller
    {
        GestBookContext db;

        public HomeController(GestBookContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            var ms =  db.Messages.Include(p => p.user);
            ViewBag.list = ms.ToList();
            return View();
        }

        public IActionResult Privacy()
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