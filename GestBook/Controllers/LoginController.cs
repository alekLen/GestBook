using Microsoft.AspNetCore.Mvc;
using GestBook.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

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
                //bool passwordsMatch = BCrypt.Net.BCrypt.Verify(enteredPassword, hashedPasswordFromDatabase); для проверки совпадения           
                user.Password = hashedPassword; 
                try
                {
                    db.Add(user);
                    await db.SaveChangesAsync();
                }
                catch { }
                return RedirectToAction("Login");
            }
            return View(user);
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user)
        {
            if (ModelState.IsValid) {
                
               var u =await  db.Users.FirstOrDefaultAsync(m => m.Name == user.Name);
              // bool u=(db.Users?.Any(e => e.Name == user.Name)).GetValueOrDefault();
                { 
                    if (u != null && BCrypt.Net.BCrypt.Verify(user.Password, u.Password))
                    {
                        HttpContext.Session.SetString("login", user.Name); // создание сессионной переменной
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        HttpContext.Session.SetString("wrongPassword", "wrong");
                        return View(user);
                    }
                }
            }
            return View(user);
        }
        public ActionResult Logout()
        {
            HttpContext.Session.Clear(); // очищается сессия
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> AddMessage([Bind("Text")]Message mes)
        {
            if (ModelState.IsValid)
            {
                var u = await db.Users.FirstOrDefaultAsync(m => m.Name == HttpContext.Session.GetString("login"));
                mes.user= u;
                mes.MessageDate=DateTime.Now.ToString();
                try
                {
                    db.Add(mes);
                    await db.SaveChangesAsync();
                }
                catch { }
                return RedirectToAction("Index", "Home");
            }
            return View(mes);
        }
    }
}
