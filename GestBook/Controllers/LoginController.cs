using Microsoft.AspNetCore.Mvc;
using GestBook.Models;
using Microsoft.EntityFrameworkCore;

namespace GestBook.Controllers
{
    public class LoginController : Controller
    {
        GestBookContext db;
        public LoginController(GestBookContext context)
        {
            db = context;
        }
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration([Bind("Name,Password")] User user)
        {          
            if (ModelState.IsValid)
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
                HttpContext.Session.SetString("login", user.Name); // создание сессионной переменной
                try
                {
                    db.Add(user);
                    await db.SaveChangesAsync();

                }
                catch { }
                return View("Login");
            }
            return View(user);
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User user)
        {
            if (ModelState.IsValid)
            {
                HttpContext.Session.SetString("login", user.Name); // создание сессионной переменной
                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }
        public ActionResult Logout()
        {
            HttpContext.Session.Clear(); // очищается сессия
            return RedirectToAction("Index", "Home");
        }
    }
}
