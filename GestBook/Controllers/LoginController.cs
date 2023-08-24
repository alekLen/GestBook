using Microsoft.AspNetCore.Mvc;
using GestBook.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

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
        public async Task<IActionResult> Registration( RegisterModel user)
        {          
            if (ModelState.IsValid)
            {
                User u = new();
                u.Name = user.Login;
                byte[] saltbuf = new byte[16];
                RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                randomNumberGenerator.GetBytes(saltbuf);
                StringBuilder sb = new StringBuilder(16);
                for (int i = 0; i < 16; i++)
                    sb.Append(string.Format("{0:X2}", saltbuf[i]));
                string salt = sb.ToString();
                Salt s = new();
                s.salt=salt;
                string password =salt + user.Password;
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                //bool passwordsMatch = BCrypt.Net.BCrypt.Verify(enteredPassword, hashedPasswordFromDatabase); для проверки совпадения           
                u.Password = hashedPassword; 
                try
                {
                    db.Add(u);
                    await db.SaveChangesAsync();
                    s.user = u;
                    db.Add(s);
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
        public async Task<IActionResult> Login(LoginModel user)
        {
            if (ModelState.IsValid) {
                
               var u =await  db.Users.FirstOrDefaultAsync(m => m.Name == user.Login);
                var s = await db.Salts.FirstOrDefaultAsync(m => m.user == u);           
                { 
                    if (u != null && s!=null)
                    {
                        string conf = s.salt + user.Password;
                        if (BCrypt.Net.BCrypt.Verify(conf, u.Password))
                        {
                            HttpContext.Session.SetString("login", user.Login); // создание сессионной переменной
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("", "не правильный логин или пароль");
                            return View(user);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "не правильный логин или пароль");
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
