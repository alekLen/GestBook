using Microsoft.AspNetCore.Mvc;
using GestBook.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;
using GestBook.Repository;

namespace GestBook.Controllers
{
    public class LoginController : Controller
    {
        IRepository rep;
        public LoginController(IRepository context)
        {
            rep = context;
        }
        /*public IActionResult Registration()
        {
            return View();
        }*/

        [HttpPost]
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
                u.Password = hashedPassword; 
                try
                {
                   await rep.AddUser(u);
                    await rep.Save();
                    s.user = u;
                    await rep.AddSalt(s);
                    await rep.Save();
                }
                catch { }
                string response = "Вы успешно зарегестрировались!";
                return Json(response);
            }
            return Problem("Проблемы регистрации!");
        }
       /* public IActionResult Login()
        {
            return View();
        }*/
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel user)
        {
            if (ModelState.IsValid) {
                
               var u =await rep.GetUser(user.Login);
                var s = await rep.GetSalt(u);           
                { 
                    if (u != null && s!=null)
                    {
                        string conf = s.salt + user.Password;
                        if (BCrypt.Net.BCrypt.Verify(conf, u.Password))
                        {
                            HttpContext.Session.SetString("login", user.Login); // создание сессионной переменной
                            string response = "Добро пожаловать, "+ user.Login;
                            return Json(response);
                        }
                        else
                        {
                            ModelState.AddModelError("", "не правильный логин или пароль");
                            return Problem("Проблемы входа!");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "не правильный логин или пароль");
                        return Problem("Проблемы входа!"); 
                    }
                }
            }
            return Problem("Проблемы входа!");
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
                var u = await rep.GetUser( HttpContext.Session.GetString("login"));
                mes.user= u;
                mes.MessageDate=DateTime.Now.ToString();
                try
                {
                    await rep.AddMessage(mes);
                    await rep.Save();
                }
                catch { }
                return RedirectToAction("Index", "Home");
            }
            return View(mes);
        }
    }
}
