using GestBook.Models;
using GestBook.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GestBook.Controllers
{
    public class HomeController : Controller
    {

        IRepository rep;

        public HomeController(IRepository context)
        {
           rep = context;
        }

        public async Task< IActionResult> Index()
        {
            var ms = await rep.GetMessage();
            ViewBag.list = ms;
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