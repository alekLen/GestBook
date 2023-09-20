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
        public IActionResult Registration()
        {
            return PartialView("Registration");
        }

        [HttpPost]
     
        public async Task<IActionResult> Registration([Bind("Login,Password,PasswordConfirm")] RegisterModel user)
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
                catch { return Json(false); }
                string response = "Вы успешно зарегестрировались!";
                return Json(response);
            }
            return Json(false);
        }
        public IActionResult Login()
        {
            return PartialView("Login");
        }
        [HttpPost]
        public async Task<IActionResult> Login([Bind("Login,Password")] LoginModel user)
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
                            //ModelState.AddModelError("", "не правильный логин или пароль");
                            //return Problem("Проблемы входа!");
                            return Json(false);
                        }
                    }
                    else
                    {
                        // ModelState.AddModelError("", "не правильный логин или пароль");
                        // return Problem("Проблемы входа!"); 
                        return Json(false);
                    }
                }
            }
            // return Problem("Проблемы входа!");
            return Json(false);
        }
        public ActionResult Logout()
        {
            string response = "Жаль, что вы так быстро покидаете нас!\n Возвращайтесь быстрее!";
            HttpContext.Session.Clear(); // очищается сессия
            return Json(response);
        }
        public ActionResult GetName()
        {
            string response = HttpContext.Session.GetString("login");
            return Json(response);
        }
        [HttpPost]
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
                    string response ="Ваш отзыв добавлен в Гостевую книгу!";
                    return Json(response);
                }
                catch
                {
                    string response1 = "Ошибка добавления! попробуйте позже!";
                    return Json(response1);
                }
                
            }
            string response2 = "Ошибка добавления! попробуйте позже!";
            return Json(response2);
        }
        [HttpPost]  
        public async Task<IActionResult> IsLoginInUse(string login)
        {
            bool isUnique = true;
            User u = await rep.GetUser(login);
            if (u == null)
                isUnique = false;
            return Json(isUnique);
        }
        public async Task<IActionResult> IsLoginIn(string login)
        {
            bool isUnique = true;
            User u = await rep.GetUser(login);
            if (u == null)
                isUnique = false;
            return Json(!isUnique);
        }
    }
}
